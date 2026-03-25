using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;

MSBuildLocator.RegisterDefaults();

// Mapa de renombres: nombre actual -> nombre nuevo
var renameMap = new Dictionary<string, string>(StringComparer.Ordinal)
{
    { "MapInfo_Renamed",    "MapInfo"    },
    { "ObjData_Renamed",    "ObjData"    },
    { "LevelSkill_Renamed", "LevelSkill" },
    { "ModClase_Renamed",   "ModClase"   },
    { "ModRaza_Renamed",    "ModRaza"    },
    { "Obj_Renamed",        "obj"        },
};

var solutionPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..", "ArgentumServer.sln"));

Console.WriteLine($"Cargando solución: {solutionPath}");

using var workspace = MSBuildWorkspace.Create();
workspace.WorkspaceFailed += (_, e) =>
    Console.WriteLine($"[WORKSPACE] {e.Diagnostic.Kind}: {e.Diagnostic.Message}");

var solution = await workspace.OpenSolutionAsync(solutionPath);

int totalRenames = 0;

foreach (var (oldName, newName) in renameMap)
{
    Console.WriteLine($"\nBuscando '{oldName}' → '{newName}'...");

    // Recargar solución actualizada del workspace después de cada rename
    solution = workspace.CurrentSolution;

    // Buscar todos los símbolos con ese nombre en todos los proyectos
    var symbols = new List<ISymbol>();

    foreach (var project in solution.Projects)
    {
        var compilation = await project.GetCompilationAsync();
        if (compilation is null) continue;

        CollectSymbols(compilation.GlobalNamespace, oldName, symbols);
    }

    if (symbols.Count == 0)
    {
        Console.WriteLine($"  No se encontró '{oldName}' — saltando.");
        continue;
    }

    // Deduplicar por SymbolKey para no renombrar el mismo símbolo dos veces
    // (puede aparecer en múltiples compilaciones si hay proyectos que se referencian)
    var seen = new HashSet<string>();
    foreach (var symbol in symbols)
    {
        var key = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (!seen.Add(key)) continue;

        Console.WriteLine($"  Renombrando: {symbol.Kind} '{symbol.ToDisplayString()}' en {symbol.Locations.FirstOrDefault()?.SourceTree?.FilePath ?? "?"}");

        solution = await Renamer.RenameSymbolAsync(solution, symbol, new SymbolRenameOptions(), newName);
        totalRenames++;
    }
}

Console.WriteLine($"\nAplicando {totalRenames} rename(s) al disco...");
if (!workspace.TryApplyChanges(solution))
{
    Console.Error.WriteLine("ERROR: TryApplyChanges falló.");
    return 1;
}

Console.WriteLine("Listo. Verificá con: dotnet build");
return 0;

// ----------------------------------------------------------------
static void CollectSymbols(INamespaceOrTypeSymbol container, string name, List<ISymbol> result)
{
    foreach (var member in container.GetMembers())
    {
        if (member.Name == name)
            result.Add(member);

        if (member is INamespaceOrTypeSymbol nested)
            CollectSymbols(nested, name, result);
    }
}
