using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace ArgentumServer.Script
{
    public class VBErrorHandlingConverter
    {
        private const string SourceFolder = @"C:\Users\Franco\Documents\AO\Argentum-Online\argentum-server-net\Legacy\Source";
        private const int FileProcessingTimeoutSeconds = 30;
        
        public void Run()
        {
            Console.WriteLine("Starting VB.NET error handling conversion...");
            
            // Get all VB files
            var vbFiles = Directory.GetFiles(SourceFolder, "*.vb", SearchOption.AllDirectories);
            Console.WriteLine($"Found {vbFiles.Length} VB files to process");
            
            int processedFiles = 0;
            int modifiedFiles = 0;
            int errorFiles = 0;
            int timeoutFiles = 0;
            
            foreach (var filePath in vbFiles)
            {
                try
                {
                    Console.WriteLine($"Processing file: {Path.GetFileName(filePath)}");
                    
                    // Create a cancellation token with timeout
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(FileProcessingTimeoutSeconds)))
                    {
                        try
                        {
                            var processTask = Task.Run(() => ProcessFile(filePath), cts.Token);
                            bool fileModified = processTask.Wait(TimeSpan.FromSeconds(FileProcessingTimeoutSeconds)) 
                                            ? processTask.Result 
                                            : false;
                            
                            if (fileModified)
                                modifiedFiles++;
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine($"WARNING: Processing timed out for file {filePath}");
                            timeoutFiles++;
                            // Try a simpler approach for timeout files
                            try
                            {
                                bool modified = ProcessFileSimple(filePath);
                                if (modified)
                                    modifiedFiles++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"ERROR: Fallback processing failed for file {filePath}: {ex.Message}");
                                errorFiles++;
                            }
                        }
                    }
                    
                    processedFiles++;
                    
                    if (processedFiles % 10 == 0)
                        Console.WriteLine($"Progress: Processed {processedFiles}/{vbFiles.Length} files, modified {modifiedFiles}, errors {errorFiles}, timeouts {timeoutFiles}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR processing file {filePath}: {ex.Message}");
                    errorFiles++;
                }
            }
            
            Console.WriteLine($"Conversion complete. Processed {processedFiles} files, modified {modifiedFiles}, errors {errorFiles}, timeouts {timeoutFiles}");
        }
        
        private bool ProcessFileSimple(string filePath)
        {
            // A simpler approach that doesn't use Roslyn, just for fallback
            string content = File.ReadAllText(filePath);
            string originalContent = content;
            bool modified = false;
            
            try
            {
                // Only process methods that contain On Error statements
                // Process On Error Resume Next
                if (content.Contains("On Error Resume Next"))
                {
                    content = ReplaceResumeNextSimple(content);
                    modified = content != originalContent;
                }
                
                // Process On Error GoTo
                if (Regex.IsMatch(content, @"On Error GoTo \w+"))
                {
                    content = ReplaceGotoSimple(content);
                    modified = content != originalContent;
                }
                
                // If modified, save the file
                if (modified)
                {
                    File.WriteAllText(filePath, content);
                    Console.WriteLine($"Modified file using simple method: {filePath}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in simple processing for {filePath}: {ex.Message}");
            }
            
            return false;
        }
        
        private string ReplaceResumeNextSimple(string content)
        {
            // Find individual methods with On Error Resume Next
            var methodPattern = @"((?:Public |Private |Friend )?(?:Sub|Function)\s+(\w+)(?:\([^)]*\))?(?:\s+As\s+\w+)?\s*\r?\n)([\s\S]*?)(End (?:Sub|Function))";
            
            return Regex.Replace(content, methodPattern, match => {
                var methodDeclaration = match.Groups[1].Value;
                var methodName = match.Groups[2].Value;
                var methodBody = match.Groups[3].Value;
                var endStatement = match.Groups[4].Value;
                
                // Only modify methods that contain "On Error Resume Next"
                if (methodBody.Contains("On Error Resume Next"))
                {
                    methodBody = methodBody.Replace("On Error Resume Next", "");
                    return $"{methodDeclaration}Try\r\n{methodBody}\r\nCatch ex As Exception\r\n    ' Error was previously ignored (On Error Resume Next)\r\n    Console.WriteLine(\"Error in {methodName}: \" & ex.Message)\r\nEnd Try\r\n{endStatement}";
                }
                
                // Return unmodified if no "On Error Resume Next"
                return match.Value;
            });
        }
        
        private string ReplaceGotoSimple(string content)
        {
            // Find individual methods with On Error GoTo
            var methodPattern = @"((?:Public |Private |Friend )?(?:Sub|Function)\s+(\w+)(?:\([^)]*\))?(?:\s+As\s+\w+)?\s*\r?\n)([\s\S]*?)(End (?:Sub|Function))";
            
            return Regex.Replace(content, methodPattern, match => {
                var methodDeclaration = match.Groups[1].Value;
                var methodName = match.Groups[2].Value;
                var methodBody = match.Groups[3].Value;
                var endStatement = match.Groups[4].Value;
                
                // Check if this method has On Error GoTo
                var gotoMatch = Regex.Match(methodBody, @"On Error GoTo\s+(\w+)");
                if (gotoMatch.Success)
                {
                    var errorLabel = gotoMatch.Groups[1].Value;
                    var labelPattern = $@"({errorLabel}:)([\s\S]*?)(?=End Sub|End Function|Exit Sub|Exit Function|\r\n\s*\r\n)";
                    var labelMatch = Regex.Match(methodBody, labelPattern);
                    
                    if (labelMatch.Success)
                    {
                        var errorHandlingCode = labelMatch.Groups[2].Value;
                        
                        // Split the method body at the label point
                        int labelIndex = methodBody.IndexOf(labelMatch.Value);
                        string beforeLabel = methodBody.Substring(0, labelIndex);
                        
                        // Remove the On Error GoTo statement
                        beforeLabel = beforeLabel.Replace(gotoMatch.Value, "");
                        
                        // Only remove Exit Sub/Function statements that appear at the end of code blocks
                        // before the error handling label - these are likely used to skip error handling in normal flow
                        beforeLabel = Regex.Replace(beforeLabel, 
                            @"((?:\r?\n)\s*)Exit\s+(Sub|Function)(\s*)(?=\r?\n)", 
                            "$1$3",  // Keep the newlines and spacing, just remove the Exit statement
                            RegexOptions.IgnoreCase);
                        
                        // Do not modify conditional Exit statements like "If condition Then Exit Sub"
                        
                        return $"{methodDeclaration}Try\r\n{beforeLabel}\r\nCatch ex As Exception\r\n    ' Error handling (previously {errorLabel})\r\n    Console.WriteLine(\"Error in {methodName}: \" & ex.Message){errorHandlingCode}\r\nEnd Try\r\n{endStatement}";
                    }
                }
                
                // Return unmodified if no "On Error GoTo" or if label not found
                return match.Value;
            });
        }
        
        private bool ProcessFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string originalContent = content;
            
            // Check file size and complexity first
            if (content.Length > 500000) // Skip extremely large files
            {
                Console.WriteLine($"File too large, using simple processing: {filePath}");
                return ProcessFileSimple(filePath);
            }
            
            // First check if the file contains any error handling statements at all
            if (!content.Contains("On Error"))
            {
                return false; // Skip files without error handling
            }
            
            try
            {
                // Parse the VB source file
                SyntaxTree tree = VisualBasicSyntaxTree.ParseText(content);
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
                
                // Find all methods with error handling
                var methodsWithErrorHandling = FindMethodsWithErrorHandling(root);
                
                if (!methodsWithErrorHandling.Any())
                    return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing file with Roslyn, falling back to simple mode: {ex.Message}");
                return ProcessFileSimple(filePath);
            }
            
            // Process the content with Regex (more reliable for this specific case)
            bool modified = false;
            
            try
            {
                // Look for methods with On Error Resume Next
                var resumeNextMatches = Regex.Matches(content, @"((?:Public |Private |Friend )?(?:Sub|Function)\s+([^\s(]+).*?\n)([\s\S]*?)(On Error Resume Next)([\s\S]*?)(End (?:Sub|Function))", RegexOptions.IgnoreCase);
                foreach (Match match in resumeNextMatches)
                {
                    string methodDeclaration = match.Groups[1].Value;
                    string methodName = match.Groups[2].Value;
                    string beforeError = match.Groups[3].Value;
                    string errorStatement = match.Groups[4].Value;
                    string methodBody = match.Groups[5].Value;
                    string endStatement = match.Groups[6].Value;
                    
                    content = content.Replace(match.Value, $"{methodDeclaration}{beforeError}Try{methodBody}\r\nCatch ex As Exception\r\n    ' Error was previously ignored (On Error Resume Next)\r\n    Console.WriteLine(\"Error in {methodName}: \" & ex.Message)\r\nEnd Try\r\n{endStatement}");
                    modified = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Resume Next in {filePath}: {ex.Message}");
            }
            
            try
            {
                // Look for methods with On Error GoTo Label
                var gotoMatches = Regex.Matches(content, @"((?:Public |Private |Friend )?(?:Sub|Function)\s+([^\s(]+).*?\n)([\s\S]*?)(On Error GoTo\s+(\w+))([\s\S]*?)(\5:)([\s\S]*?)(End (?:Sub|Function))", RegexOptions.IgnoreCase);
                foreach (Match match in gotoMatches)
                {
                    string methodDeclaration = match.Groups[1].Value;
                    string methodName = match.Groups[2].Value;
                    string beforeError = match.Groups[3].Value;
                    string errorStatement = match.Groups[4].Value;
                    string errorLabel = match.Groups[5].Value;
                    string methodBody = match.Groups[6].Value;
                    string errorLabelDef = match.Groups[7].Value;
                    string errorHandlingCode = match.Groups[8].Value;
                    string endStatement = match.Groups[9].Value;
                    
                    // Only remove Exit Sub/Function statements that appear at the end of code blocks
                    // before the error handling label - these are likely used to skip error handling
                    methodBody = Regex.Replace(methodBody, 
                        @"((?:\r?\n)\s*)Exit\s+(Sub|Function)(\s*)(?=\r?\n)", 
                        "$1$3",  // Keep the newlines and spacing, just remove the Exit statement
                        RegexOptions.IgnoreCase);
                    
                    // Do not modify conditional Exit statements like "If condition Then Exit Sub"
                    
                    // Create new method with Try-Catch
                    string newMethod = $"{methodDeclaration}{beforeError}Try{methodBody}\r\nCatch ex As Exception\r\n    ' Error handling (previously {errorLabel})\r\n    Console.WriteLine(\"Error in {methodName}: \" & ex.Message){errorHandlingCode}\r\nEnd Try\r\n{endStatement}";
                    
                    content = content.Replace(match.Value, newMethod);
                    modified = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing GoTo in {filePath}: {ex.Message}");
            }
            
            // If modified, save the file
            if (modified && content != originalContent)
            {
                File.WriteAllText(filePath, content);
                Console.WriteLine($"Modified file: {filePath}");
                return true;
            }
            
            return false;
        }
        
        private IEnumerable<MethodBlockSyntax> FindMethodsWithErrorHandling(CompilationUnitSyntax root)
        {
            // Get all method blocks
            var methodBlocks = root.DescendantNodes().OfType<MethodBlockSyntax>();
            
            // Filter to those containing On Error statements
            var methodsWithErrorHandling = methodBlocks.Where(method => 
                method.DescendantNodes()
                    .OfType<StatementSyntax>()
                    .Any(stmt => stmt.ToString().Contains("On Error")));
            
            return methodsWithErrorHandling;
        }
    }
    
    public class Script
    {
        public static void Main()
        {
            Console.WriteLine("Error Handling Migration Script");
            Console.WriteLine("==============================");
            Console.WriteLine("This script will convert old VB.NET error handling to Try-Catch blocks");
            Console.WriteLine("in all .vb files in the Legacy/Source directory");
            Console.WriteLine();
            
            try
            {
                var converter = new VBErrorHandlingConverter();
                converter.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running script: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Script execution complete. Press any key to exit...");
            Console.ReadKey();
        }
    }
}