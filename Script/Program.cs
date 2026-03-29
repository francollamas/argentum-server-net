using WarningsFixerTools;

namespace ArgentumServer.WarningsFixer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Roslyn Warnings Fixer ===");
        Console.WriteLine();

        // Configuración
        const bool DRY_RUN = false;
        const bool VERBOSE = true;
        
        var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
        var solutionPath = Path.Combine(basePath, "ArgentumServer.sln");
        var reportPath = Path.Combine(basePath, "docs/WARNINGS_CLEANUP_REPORT.md");

        if (!File.Exists(solutionPath))
        {
            Console.WriteLine($"❌ Solución no encontrada: {solutionPath}");
            return;
        }

        try
        {
            // Cargar solución
            var solution = await CodeAnalysisUtils.LoadSolutionAsync(solutionPath);
            var report = new WarningsReport();

            // Fase 1: Variables no usadas (CS0168 + CS0219)
            Console.WriteLine();
            Console.WriteLine("--- Fase 1: Variables no usadas ---");
            var variablesFixer = new UnusedVariablesFixer(solution, report, basePath);
            solution = await variablesFixer.FixUnusedVariablesAsync();

            // Fase 2: Fields no usados (CS0169 + CS0649)
            Console.WriteLine();
            Console.WriteLine("--- Fase 2: Fields no usados ---");
            var fieldsFixer = new UnusedFieldsFixer(solution, report, basePath);
            solution = await fieldsFixer.FixUnusedFieldsAsync();

            // Fase 3: Type naming (CS8981)
            Console.WriteLine();
            Console.WriteLine("--- Fase 3: Type naming ---");
            var typeNamingFixer = new TypeNamingFixer(solution, report, basePath);
            solution = await typeNamingFixer.FixTypeNamingAsync();

            // Aplicar cambios
            if (!DRY_RUN)
            {
                Console.WriteLine();
                Console.WriteLine("Guardando cambios a disco...");
                
                // Siempre guardamos manualmente porque TryApplyChanges solo modifica en memoria
                foreach (var projectId in solution.ProjectIds)
                {
                    var project = solution.GetProject(projectId);
                    if (project == null) continue;

                    foreach (var documentId in project.DocumentIds)
                    {
                        var document = solution.GetDocument(documentId);
                        if (document == null) continue;

                        var root = await document.GetSyntaxRootAsync();
                        if (root == null) continue;

                        var text = root.ToFullString();
                        var filePath = document.FilePath;
                        
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            await File.WriteAllTextAsync(filePath, text);
                            if (VERBOSE)
                                Console.WriteLine($"  ✓ {Path.GetFileName(filePath)}");
                        }
                    }
                }
                
                Console.WriteLine("✓ Cambios guardados exitosamente");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("⚠️  DRY RUN MODE - No se guardaron cambios");
            }

            // Generar reporte
            Console.WriteLine();
            Console.WriteLine("Generando reporte...");
            report.GenerateReport(reportPath);

            Console.WriteLine();
            Console.WriteLine($"=== Total warnings corregidas: {report.TotalWarningsFixed}/82 ===");
            Console.WriteLine();
            Console.WriteLine("✓ Proceso completado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
}
