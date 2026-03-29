using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;

namespace WarningsFixerTools;

public class TypeNamingFixer
{
    private readonly Solution _solution;
    private readonly WarningsReport _report;
    private readonly string _basePath;

    public TypeNamingFixer(Solution solution, WarningsReport report, string basePath)
    {
        _solution = solution;
        _report = report;
        _basePath = basePath;
    }

    public async Task<Solution> FixTypeNamingAsync()
    {
        var currentSolution = _solution;
        int typesRenamed = 0;

        // Buscar el tipo 'npc' en Declares.cs
        foreach (var project in currentSolution.Projects)
        {
            foreach (var document in project.Documents)
            {
                if (!document.FilePath?.Contains("Declares.cs") ?? true)
                    continue;

                var semanticModel = await document.GetSemanticModelAsync();
                var root = await document.GetSyntaxRootAsync();
                
                if (semanticModel == null || root == null)
                    continue;

                // Buscar declaraciones de clase con nombre lowercase
                var classDeclarations = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(c => c.Identifier.Text == "npc")
                    .ToList();

                foreach (var classDecl in classDeclarations)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(classDecl);
                    if (symbol == null)
                        continue;

                    var lineNumber = classDecl.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                    var filePath = document.FilePath ?? "unknown";
                    var relativePath = CodeAnalysisUtils.GetRelativePath(filePath, _basePath);

                    Console.WriteLine($"  → Renaming type 'npc' to 'Npc' at {relativePath}:{lineNumber}");

                    // Usar Roslyn Renamer para renombrar TODAS las referencias
                    currentSolution = await Renamer.RenameSymbolAsync(
                        currentSolution,
                        symbol,
                        "Npc",
                        default(Microsoft.CodeAnalysis.Options.OptionSet)
                    );

                    _report.AddChange("CS8981", relativePath, lineNumber, 
                        "Renamed type 'npc' to 'Npc' (including all references)");
                    typesRenamed++;
                }
            }
        }

        if (typesRenamed > 0)
        {
            Console.WriteLine($"✓ CS8981: {typesRenamed} tipo(s) renombrado(s) a PascalCase");
        }

        return currentSolution;
    }
}
