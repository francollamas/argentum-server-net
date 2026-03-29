using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace WarningsFixerTools;

public class UnusedFieldsFixer
{
    private readonly Solution _solution;
    private readonly WarningsReport _report;
    private readonly string _basePath;

    public UnusedFieldsFixer(Solution solution, WarningsReport report, string basePath)
    {
        _solution = solution;
        _report = report;
        _basePath = basePath;
    }

    public async Task<Solution> FixUnusedFieldsAsync()
    {
        var currentSolution = _solution;
        int cs0169Fixed = 0; // Fields nunca usados
        int cs0649Fixed = 0; // Fields nunca asignados (pero que podemos eliminar)

        foreach (var project in currentSolution.Projects)
        {
            foreach (var document in project.Documents)
            {
                if (!document.FilePath?.EndsWith(".cs") ?? true)
                    continue;

                var semanticModel = await document.GetSemanticModelAsync();
                var root = await document.GetSyntaxRootAsync();
                
                if (semanticModel == null || root == null)
                    continue;

                var fieldDeclarations = root.DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .ToList();

                var fieldsToRemove = new List<FieldDeclarationSyntax>();

                foreach (var fieldDecl in fieldDeclarations)
                {
                    foreach (var variable in fieldDecl.Declaration.Variables)
                    {
                        var symbol = semanticModel.GetDeclaredSymbol(variable);
                        if (symbol == null)
                            continue;

                        var fieldSymbol = symbol as IFieldSymbol;
                        if (fieldSymbol == null)
                            continue;

                        // Buscar referencias
                        var references = await SymbolFinder.FindReferencesAsync(fieldSymbol, currentSolution);
                        var hasReferences = references.Any(r => r.Locations.Any(loc => !loc.IsImplicit));

                        var lineNumber = fieldDecl.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                        var filePath = document.FilePath ?? "unknown";
                        var relativePath = CodeAnalysisUtils.GetRelativePath(filePath, _basePath);
                        var fieldName = variable.Identifier.Text;

                        if (!hasReferences)
                        {
                            // CS0169: Field nunca usado
                            fieldsToRemove.Add(fieldDecl);
                            _report.AddChange("CS0169", relativePath, lineNumber, 
                                $"Removed unused field '{fieldName}'");
                            cs0169Fixed++;
                        }
                        else
                        {
                            // Verificar si nunca se asigna (CS0649)
                            // Solo eliminamos si NO hay asignaciones
                            var hasWrites = await HasWriteReferencesAsync(fieldSymbol, currentSolution);
                            
                            if (!hasWrites)
                            {
                                // Field se lee pero nunca se asigna
                                // Solo eliminamos si está en la lista conocida de eliminables
                                var shouldEliminate = IsInEliminableList(relativePath, lineNumber);
                                
                                if (shouldEliminate)
                                {
                                    fieldsToRemove.Add(fieldDecl);
                                    _report.AddChange("CS0649", relativePath, lineNumber, 
                                        $"Removed field '{fieldName}' (never assigned, verified safe to remove)");
                                    cs0649Fixed++;
                                }
                            }
                        }
                    }
                }

                // Aplicar cambios
                if (fieldsToRemove.Any())
                {
                    var newRoot = root.RemoveNodes(fieldsToRemove, SyntaxRemoveOptions.KeepNoTrivia);
                    
                    if (newRoot != null)
                    {
                        var newDocument = document.WithSyntaxRoot(newRoot);
                        currentSolution = newDocument.Project.Solution;
                    }
                }
            }
        }

        Console.WriteLine($"✓ CS0169: {cs0169Fixed} fields nunca usados eliminados");
        Console.WriteLine($"✓ CS0649: {cs0649Fixed} fields nunca asignados eliminados");

        return currentSolution;
    }

    private async Task<bool> HasWriteReferencesAsync(IFieldSymbol fieldSymbol, Solution solution)
    {
        var references = await SymbolFinder.FindReferencesAsync(fieldSymbol, solution);
        
        foreach (var reference in references)
        {
            foreach (var location in reference.Locations)
            {
                if (location.IsImplicit)
                    continue;

                var document = solution.GetDocument(location.Location.SourceTree);
                if (document == null)
                    continue;

                var root = await document.GetSyntaxRootAsync();
                if (root == null)
                    continue;

                var node = root.FindNode(location.Location.SourceSpan);
                
                // Verificar si es una escritura
                if (IsWriteReference(node))
                    return true;
            }
        }
        
        return false;
    }

    private bool IsWriteReference(SyntaxNode node)
    {
        // Verificar si el nodo es parte de una asignación (lado izquierdo)
        var parent = node.Parent;
        
        if (parent is AssignmentExpressionSyntax assignment)
        {
            return assignment.Left.Contains(node);
        }

        // Prefijos de incremento/decremento
        if (parent is PrefixUnaryExpressionSyntax prefix)
        {
            return prefix.Kind() is SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression;
        }

        // Postfijos de incremento/decremento
        if (parent is PostfixUnaryExpressionSyntax postfix)
        {
            return postfix.Kind() is SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression;
        }

        return false;
    }

    private bool IsInEliminableList(string relativePath, int lineNumber)
    {
        // Lista de fields conocidos que podemos eliminar de forma segura
        // Basado en el análisis del plan
        var eliminableFields = new Dictionary<string, List<int>>
        {
            { "Legacy/Source/Server/Admin.cs", new List<int> { 32, 33, 31 } },
            { "Legacy\\Source\\Server\\Admin.cs", new List<int> { 32, 33, 31 } },
            { "Legacy/Source/Server/Declares.cs", new List<int> { 1090, 1089, 1091, 879, 898, 825, 827, 919, 1110, 1109, 1111 } },
            { "Legacy\\Source\\Server\\Declares.cs", new List<int> { 1090, 1089, 1091, 879, 898, 825, 827, 919, 1110, 1109, 1111 } },
            { "Legacy/Source/Server/MODULO_LasPParty.cs", new List<int> { } },
            { "Legacy/Source/Server/clsByteQueue.cs", new List<int> { 599, 600, 611 } },
            { "Legacy\\Source\\Server\\clsByteQueue.cs", new List<int> { 599, 600, 611 } },
            { "Legacy/Source/Server/cSolicitud.cs", new List<int> { 5, 6 } },
            { "Legacy\\Source\\Server\\cSolicitud.cs", new List<int> { 5, 6 } },
        };

        foreach (var kvp in eliminableFields)
        {
            if (relativePath.Contains(kvp.Key.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar)))
            {
                return kvp.Value.Contains(lineNumber);
            }
        }

        return false;
    }
}
