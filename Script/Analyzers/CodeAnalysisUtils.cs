using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Text;

namespace WarningsFixerTools;

public static class CodeAnalysisUtils
{
    public static async Task<Solution> LoadSolutionAsync(string solutionPath)
    {
        Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
        
        var workspace = Microsoft.CodeAnalysis.MSBuild.MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (sender, e) =>
        {
            if (e.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
            {
                Console.WriteLine($"⚠️  Workspace error: {e.Diagnostic.Message}");
            }
        };

        Console.WriteLine($"Cargando solución: {solutionPath}");
        var solution = await workspace.OpenSolutionAsync(solutionPath);
        Console.WriteLine($"✓ Solución cargada: {solution.Projects.Count()} proyectos");
        
        return solution;
    }

    public static async Task<bool> HasReferencesAsync(ISymbol symbol, Solution solution)
    {
        var references = await SymbolFinder.FindReferencesAsync(symbol, solution);
        return references.Any(r => r.Locations.Any());
    }

    public static string GetMethodName(SyntaxNode node)
    {
        var method = node.AncestorsAndSelf()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault();
        
        if (method != null)
            return method.Identifier.Text;

        var ctor = node.AncestorsAndSelf()
            .OfType<ConstructorDeclarationSyntax>()
            .FirstOrDefault();

        if (ctor != null)
            return ".ctor";

        var property = node.AncestorsAndSelf()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault();

        if (property != null)
            return property.Identifier.Text;

        return "Unknown";
    }

    public static string GetClassName(SyntaxNode node)
    {
        var classDecl = node.AncestorsAndSelf()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        
        return classDecl?.Identifier.Text ?? "GlobalScope";
    }

    public static bool IsInCatchBlock(LocalDeclarationStatementSyntax localDecl)
    {
        return localDecl.Ancestors().OfType<CatchClauseSyntax>().Any();
    }

    public static CatchClauseSyntax? GetParentCatchBlock(LocalDeclarationStatementSyntax localDecl)
    {
        return localDecl.Ancestors().OfType<CatchClauseSyntax>().FirstOrDefault();
    }

    public static bool IsCatchVariableDeclaration(LocalDeclarationStatementSyntax localDecl)
    {
        var variable = localDecl.Declaration.Variables.FirstOrDefault();
        if (variable == null) return false;

        var varName = variable.Identifier.Text;
        
        // Variables comunes en catch: ex, e, exception, err, error
        return varName.Equals("ex", StringComparison.OrdinalIgnoreCase) ||
               varName.Equals("e", StringComparison.OrdinalIgnoreCase) ||
               varName.Equals("exception", StringComparison.OrdinalIgnoreCase) ||
               varName.Equals("err", StringComparison.OrdinalIgnoreCase) ||
               varName.Equals("error", StringComparison.OrdinalIgnoreCase);
    }

    public static string GenerateExceptionLogStatement(string className, string methodName, string exceptionVarName)
    {
        return $"Console.WriteLine($\"Exception in {className}.{methodName}: {{{exceptionVarName}.Message}}\");";
    }

    public static async Task<int> CountReferencesAsync(ISymbol symbol, Solution solution)
    {
        var references = await SymbolFinder.FindReferencesAsync(symbol, solution);
        return references.Sum(r => r.Locations.Count());
    }

    public static string GetRelativePath(string fullPath, string basePath)
    {
        var fullUri = new Uri(fullPath);
        var baseUri = new Uri(basePath.EndsWith(Path.DirectorySeparatorChar.ToString()) 
            ? basePath 
            : basePath + Path.DirectorySeparatorChar);
        
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString()
            .Replace('/', Path.DirectorySeparatorChar));
    }

    public static string FormatFileLocation(string filePath, int lineNumber, string basePath)
    {
        var relativePath = GetRelativePath(filePath, basePath);
        return $"{relativePath}:{lineNumber}";
    }
}

public class WarningsReport
{
    private readonly StringBuilder _report = new();
    private readonly List<string> _changes = new();
    private int _totalWarningsFixed = 0;
    
    public void AddChange(string category, string file, int line, string description)
    {
        var change = $"  - {category} @ {file}:{line} - {description}";
        _changes.Add(change);
        _totalWarningsFixed++;
    }

    public void AddSection(string title)
    {
        _report.AppendLine();
        _report.AppendLine($"## {title}");
        _report.AppendLine();
    }

    public void AddInfo(string info)
    {
        _report.AppendLine(info);
    }

    public void GenerateReport(string outputPath)
    {
        var finalReport = new StringBuilder();
        
        finalReport.AppendLine("# 📊 WARNINGS CLEANUP REPORT");
        finalReport.AppendLine();
        finalReport.AppendLine($"**Fecha:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        finalReport.AppendLine($"**Total Warnings Corregidas:** {_totalWarningsFixed}");
        finalReport.AppendLine();
        finalReport.AppendLine("---");
        finalReport.AppendLine();
        
        finalReport.Append(_report);
        
        finalReport.AppendLine();
        finalReport.AppendLine("## 📋 LISTA COMPLETA DE CAMBIOS");
        finalReport.AppendLine();
        
        foreach (var change in _changes)
        {
            finalReport.AppendLine(change);
        }
        
        File.WriteAllText(outputPath, finalReport.ToString());
        Console.WriteLine($"✓ Reporte generado: {outputPath}");
    }

    public int TotalWarningsFixed => _totalWarningsFixed;
}
