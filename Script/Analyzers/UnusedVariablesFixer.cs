using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace WarningsFixerTools;

public class UnusedVariablesFixer
{
    private readonly Solution _solution;
    private readonly WarningsReport _report;
    private readonly string _basePath;

    public UnusedVariablesFixer(Solution solution, WarningsReport report, string basePath)
    {
        _solution = solution;
        _report = report;
        _basePath = basePath;
    }

    public async Task<Solution> FixUnusedVariablesAsync()
    {
        var currentSolution = _solution;
        int cs0168Fixed = 0;
        int cs0219Fixed = 0;
        int catchBlocksUpdated = 0;

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

                var localDeclarations = root.DescendantNodes()
                    .OfType<LocalDeclarationStatementSyntax>()
                    .ToList();

                var nodesToRemove = new List<SyntaxNode>();
                var catchBlocksToUpdate = new Dictionary<CatchClauseSyntax, List<string>>();

                foreach (var localDecl in localDeclarations)
                {
                    var variable = localDecl.Declaration.Variables.FirstOrDefault();
                    if (variable == null)
                        continue;

                    var symbol = semanticModel.GetDeclaredSymbol(variable);
                    if (symbol == null)
                        continue;

                    // Buscar referencias en el documento actual
                    var references = await SymbolFinder.FindReferencesAsync(symbol, _solution);
                    var hasReferences = references.Any(r => r.Locations.Any(loc => !loc.IsImplicit));

                    if (!hasReferences)
                    {
                        var lineNumber = localDecl.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                        var filePath = document.FilePath ?? "unknown";
                        var relativePath = CodeAnalysisUtils.GetRelativePath(filePath, _basePath);

                        // Caso especial: variables en catch blocks
                        var catchBlock = CodeAnalysisUtils.GetParentCatchBlock(localDecl);
                        if (catchBlock != null && CodeAnalysisUtils.IsCatchVariableDeclaration(localDecl))
                        {
                            var exceptionVarName = variable.Identifier.Text;

                            if (!catchBlocksToUpdate.ContainsKey(catchBlock))
                                catchBlocksToUpdate[catchBlock] = new List<string>();

                            catchBlocksToUpdate[catchBlock].Add(exceptionVarName);
                            
                            _report.AddChange("CS0168", relativePath, lineNumber, 
                                $"Added exception logging for '{exceptionVarName}' in catch block");
                            catchBlocksUpdated++;
                        }
                        else
                        {
                            // Variable normal no usada
                            nodesToRemove.Add(localDecl);
                            
                            var isAssigned = variable.Initializer != null;
                            var warningCode = isAssigned ? "CS0219" : "CS0168";
                            var varName = variable.Identifier.Text;
                            
                            _report.AddChange(warningCode, relativePath, lineNumber, 
                                $"Removed unused variable '{varName}'");
                            
                            if (warningCode == "CS0219")
                                cs0219Fixed++;
                            else
                                cs0168Fixed++;
                        }
                    }
                }

                // Aplicar cambios si hay algo que hacer
                if (nodesToRemove.Any() || catchBlocksToUpdate.Any())
                {
                    var newRoot = root;

                    // Remover variables no usadas
                    newRoot = newRoot.RemoveNodes(nodesToRemove, SyntaxRemoveOptions.KeepNoTrivia);

                    // Actualizar catch blocks con logging
                    foreach (var kvp in catchBlocksToUpdate)
                    {
                        var catchBlock = kvp.Key;
                        var exceptionVarNames = kvp.Value;

                        var className = CodeAnalysisUtils.GetClassName(catchBlock);
                        var methodName = CodeAnalysisUtils.GetMethodName(catchBlock);

                        // Buscar el catch block en el nuevo árbol
                        var currentCatchBlock = newRoot?.DescendantNodes()
                            .OfType<CatchClauseSyntax>()
                            .FirstOrDefault(c => c.SpanStart == catchBlock.SpanStart);

                        if (currentCatchBlock?.Block != null)
                        {
                            foreach (var exVarName in exceptionVarNames)
                            {
                                var logStatement = CodeAnalysisUtils.GenerateExceptionLogStatement(
                                    className, methodName, exVarName);
                                
                                var logSyntax = SyntaxFactory.ParseStatement(logStatement)
                                    .WithLeadingTrivia(SyntaxFactory.Whitespace("    "));

                                var newBlock = currentCatchBlock.Block.WithStatements(
                                    currentCatchBlock.Block.Statements.Insert(0, logSyntax));

                                newRoot = newRoot?.ReplaceNode(currentCatchBlock.Block, newBlock);
                            }
                        }
                    }

                    if (newRoot != null)
                    {
                        var newDocument = document.WithSyntaxRoot(newRoot);
                        currentSolution = newDocument.Project.Solution;
                    }
                }
            }
        }

        Console.WriteLine($"✓ CS0168: {cs0168Fixed} variables no usadas eliminadas");
        Console.WriteLine($"  - {catchBlocksUpdated} catch blocks actualizados con logging");
        Console.WriteLine($"✓ CS0219: {cs0219Fixed} variables asignadas no usadas eliminadas");

        return currentSolution;
    }
}
