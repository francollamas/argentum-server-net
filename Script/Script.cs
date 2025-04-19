using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var folder = @"C:\Users\Franco\Documents\AO\Argentum-Online\argentum-server-net\Legacy\Source";
        int modifiedCount = 0;  // Contador de archivos modificados

        foreach (var file in Directory.GetFiles(folder, "*.vb", SearchOption.AllDirectories))
        {
            var code = File.ReadAllText(file);
            var tree = VisualBasicSyntaxTree.ParseText(code);
            var root = tree.GetRoot();

            var rewriter = new OnErrorRewriter();
            var newRoot = rewriter.Visit(root);

            if (!newRoot.IsEquivalentTo(root))
            {
                File.WriteAllText(file, newRoot.ToFullString());
                modifiedCount++;  // Incrementar contador cuando se modifica el archivo
                Console.WriteLine($"Modificado: {file}");
            }
        }

        Console.WriteLine($"Proceso completado. Archivos modificados: {modifiedCount}");
    }
}

class OnErrorRewriter : VisualBasicSyntaxRewriter
{
    public override SyntaxNode VisitMethodBlock(MethodBlockSyntax node)
    {
        var statements = node.Statements;
        var newStatements = SyntaxFactory.List<StatementSyntax>();

        bool insideOnError = false;
        var tryBlock = SyntaxFactory.List<StatementSyntax>();

        foreach (var stmt in statements)
        {
            var kind = stmt.Kind();

            if (kind == SyntaxKind.OnErrorResumeNextStatement || kind == SyntaxKind.OnErrorGoToLabelStatement)
            {
                insideOnError = true;
                continue; // Omitimos "On Error"
            }

            if (insideOnError)
            {
                if (IsEndOfBlock(stmt))
                {
                    var tryStmt = BuildTryCatch(tryBlock);
                    newStatements = newStatements.Add(tryStmt);
                    tryBlock = SyntaxFactory.List<StatementSyntax>();
                    insideOnError = false;

                    newStatements = newStatements.Add(stmt);
                }
                else
                {
                    tryBlock = tryBlock.Add(stmt);
                }
            }
            else
            {
                newStatements = newStatements.Add(stmt);
            }
        }

        if (insideOnError && tryBlock.Any())
        {
            var tryStmt = BuildTryCatch(tryBlock);
            newStatements = newStatements.Add(tryStmt);
        }

        return node.WithStatements(newStatements);
    }

    private TryBlockSyntax BuildTryCatch(SyntaxList<StatementSyntax> tryStatements)
    {
        var filteredStatements = FilterErrNumberAndClear(tryStatements);

        // Comentario como trivia en una línea vacía
        var emptyLineWithComment = SyntaxFactory.EmptyStatement()
            .WithLeadingTrivia(SyntaxFactory.CommentTrivia("' TODO: manejar error con ex.Message o ex.StackTrace"));

        var catchBlock = SyntaxFactory.CatchBlock(
            SyntaxFactory.CatchStatement(
                SyntaxFactory.IdentifierName("ex"),
                SyntaxFactory.SimpleAsClause(SyntaxFactory.ParseTypeName("Exception")),
                null
            ),
            SyntaxFactory.SingletonList<StatementSyntax>(emptyLineWithComment)
        );

        return SyntaxFactory.TryBlock(
            filteredStatements,
            SyntaxFactory.SingletonList(catchBlock),
            null
        );
    }

    private SyntaxList<StatementSyntax> FilterErrNumberAndClear(SyntaxList<StatementSyntax> statements)
    {
        var newList = SyntaxFactory.List<StatementSyntax>();

        foreach (var stmt in statements)
        {
            // Elimina bloques: If Err.Number <> 0 Then ...
            if (stmt is MultiLineIfBlockSyntax ifBlock)
            {
                var condition = ifBlock.IfStatement.Condition.ToString().Replace(" ", "").ToLower();
                if (condition.Contains("err.number<>0") || condition.Contains("err.number!=0"))
                    continue;
            }

            // Elimina If simples
            if (stmt is IfStatementSyntax singleIf)
            {
                var condition = singleIf.Condition.ToString().Replace(" ", "").ToLower();
                if (condition.Contains("err.number<>0") || condition.Contains("err.number!=0"))
                    continue;
            }

            // Elimina Err.Clear()
            var stmtText = stmt.ToString().Replace(" ", "").ToLower();
            if (stmtText.StartsWith("err.clear()") || stmtText.StartsWith("callerr.clear()"))
                continue;

            newList = newList.Add(stmt);
        }

        return newList;
    }

    private bool IsEndOfBlock(StatementSyntax stmt)
    {
        return stmt is LabelStatementSyntax || stmt is ReturnStatementSyntax || stmt is ExitStatementSyntax;
    }
}
