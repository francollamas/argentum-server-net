/// <summary>
/// Fix script: reemplaza `Err.Number`, `Err.Description`, `Err.Source`, `Err.HelpFile`,
/// `Err.HelpContext` y `Err.LastDllError` dentro de bloques `Catch` en el proyecto VB.NET.
///
/// Transformaciones aplicadas (usando el nombre de la variable de catch del bloque):
///   Err.Number      → ex.GetType().Name
///   Err.Description → ex.Message
///   Err.Source      → ex.Message
///   Err.HelpFile    → ex.Message
///   Err.HelpContext → ex.Message
///   Err.LastDllError→ ex.Message
///
/// IMPORTANTE: Solo transforma usos que sean descendientes directos de un CatchBlockSyntax.
/// Los usos de Err.* fuera de Catch (ej: Protocol.vb control flow VB6 COM) NO son tocados.
///
/// Uso:
///   dotnet run --project Script/roslyn/fix-err-number -- <path-to-vbproj>
/// Ejemplo:
///   dotnet run --project Script/roslyn/fix-err-number -- Legacy/Legacy.vbproj
/// </summary>

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

MSBuildLocator.RegisterDefaults();

if (args.Length == 0)
{
    Console.Error.WriteLine("Uso: dotnet run -- <path-to-.vbproj>");
    return 1;
}

var projectPath = Path.GetFullPath(args[0]);
if (!File.Exists(projectPath))
{
    Console.Error.WriteLine($"No se encontró el proyecto: {projectPath}");
    return 1;
}

Console.WriteLine($"Cargando proyecto: {projectPath}");
Console.WriteLine();

using var workspace = MSBuildWorkspace.Create();
workspace.WorkspaceFailed += (_, e) =>
    Console.Error.WriteLine($"[WorkspaceWarning] {e.Diagnostic.Message}");

var project = await workspace.OpenProjectAsync(projectPath);

int totalFiles = 0;
int totalReplacements = 0;

// Las propiedades del objeto Err que reemplazamos dentro de Catch
var errProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    "Number", "Description", "Source", "HelpFile", "HelpContext", "LastDllError"
};

foreach (var doc in project.Documents)
{
    if (!doc.FilePath!.EndsWith(".vb", StringComparison.OrdinalIgnoreCase))
        continue;

    var tree = await doc.GetSyntaxTreeAsync();
    var root = await tree!.GetRootAsync() as VisualBasicSyntaxNode;
    if (root is null) continue;

    var rewriter = new ErrRewriter(errProperties);
    var newRoot = rewriter.Visit(root);

    if (rewriter.ReplacementCount > 0)
    {
        totalFiles++;
        totalReplacements += rewriter.ReplacementCount;

        var newSource = newRoot!.ToFullString();
        await File.WriteAllTextAsync(doc.FilePath!, newSource);

        Console.WriteLine($"  [{Path.GetFileName(doc.FilePath!)}] — {rewriter.ReplacementCount} reemplazos");
        foreach (var detail in rewriter.Details)
            Console.WriteLine($"      línea {detail.Line,5}: {detail.Original,-50} → {detail.Replacement}");
        Console.WriteLine();
    }
}

Console.WriteLine("══════════════════════════════════════════════════════════════════");
Console.WriteLine($"  Total archivos modificados : {totalFiles}");
Console.WriteLine($"  Total reemplazos aplicados : {totalReplacements}");
Console.WriteLine("══════════════════════════════════════════════════════════════════");

return 0;

// ─────────────────────────────────────────────────────────────────────────────
// Rewriter principal
// ─────────────────────────────────────────────────────────────────────────────

class ErrRewriter : VisualBasicSyntaxRewriter
{
    private readonly HashSet<string> _errProperties;
    public int ReplacementCount { get; private set; }
    public List<ReplacementDetail> Details { get; } = [];

    // Estado que se actualiza al entrar/salir de un CatchBlock
    private string? _currentCatchVar;

    public ErrRewriter(HashSet<string> errProperties) : base(visitIntoStructuredTrivia: false)
    {
        _errProperties = errProperties;
    }

    public override SyntaxNode? VisitCatchBlock(CatchBlockSyntax node)
    {
        // Obtener el nombre de la variable de catch (ej: "ex")
        // La sintaxis es: Catch <identifier> As <type>
        var catchStatement = node.CatchStatement;
        string catchVar = catchStatement.IdentifierName?.Identifier.ValueText ?? "ex";

        var previous = _currentCatchVar;
        _currentCatchVar = catchVar;

        var result = base.VisitCatchBlock(node);

        _currentCatchVar = previous;
        return result;
    }

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        // Solo actuamos dentro de un Catch (catchVar != null)
        if (_currentCatchVar is null)
            return base.VisitMemberAccessExpression(node);

        // Nodos sintéticos (creados por SyntaxFactory en esta misma pasada) no tienen árbol
        // asociado — los ignoramos para evitar recursión sobre nuestros propios reemplazos.
        if (node.SyntaxTree is null)
            return node;

        // Verificar que sea Err.<algo>
        // node.Expression puede ser null en bloques With (acceso implícito ".Prop")
        if (node.Expression is null)
            return base.VisitMemberAccessExpression(node);

        var objName = node.Expression.ToString().Trim();
        if (!string.Equals(objName, "Err", StringComparison.OrdinalIgnoreCase))
            return base.VisitMemberAccessExpression(node);

        var propName = node.Name.Identifier.ValueText;
        if (!_errProperties.Contains(propName))
            return base.VisitMemberAccessExpression(node);

        // Determinar el reemplazo
        string replacement = propName.ToLowerInvariant() switch
        {
            "number"       => $"{_currentCatchVar}.GetType().Name",
            "description"  => $"{_currentCatchVar}.Message",
            "source"       => $"{_currentCatchVar}.Message",
            "helpfile"     => $"{_currentCatchVar}.Message",
            "helpcontext"  => $"{_currentCatchVar}.Message",
            "lastdllerror" => $"{_currentCatchVar}.Message",
            _              => $"{_currentCatchVar}.Message",
        };

        // Registrar detalle para el log — usar el árbol del nodo original
        int lineNumber = -1;
        var nodeTree = node.SyntaxTree;
        if (nodeTree is not null)
        {
            var lineSpan = nodeTree.GetLineSpan(node.Span);
            lineNumber = lineSpan.StartLinePosition.Line + 1;
        }

        ReplacementCount++;
        Details.Add(new ReplacementDetail(
            Line: lineNumber,
            Original: $"Err.{propName}",
            Replacement: replacement
        ));

        // Parsear el reemplazo como expresión VB preservando el trivia original
        var leadingTrivia  = node.GetLeadingTrivia();
        var trailingTrivia = node.GetTrailingTrivia();

        var newExpr = SyntaxFactory.ParseExpression(replacement)
            .WithLeadingTrivia(leadingTrivia)
            .WithTrailingTrivia(trailingTrivia);

        return newExpr;
    }
}

record ReplacementDetail(int Line, string Original, string Replacement);
