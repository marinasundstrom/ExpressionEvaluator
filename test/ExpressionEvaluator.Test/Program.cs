using ExpressionEvaluator.CodeGen;
using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SemanticAnalysis;
using ExpressionEvaluator.SyntaxAnalysis;
using ExpressionEvaluator.Utilites;
using System;
using System.Linq;

namespace ExpressionEvaluator.Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var input =
@"let fib n = 
    if n <= 2 then
        1
    else
        fib(n - 1) + fib(n - 2)

fib(40);
";
            using (var reader = StringHelpers.TextReaderFromString(input))
            {
                var lexer = new Lexer(reader);

                //while (!lexer.IsEof)
                //{
                //    var t = lexer.ReadToken();
                //    Console.WriteLine(t.Kind);
                //}

                var parser = new Parser(lexer);

                var expression = parser.Parse();

                var model = new SemanticModel(parser.Diagnostics);
                var expressionInfo = model.GetExpressionInfo(expression);

                if (parser.Diagnostics.Any())
                {
                    foreach (var diagnostic in parser.Diagnostics)
                    {
                        Console.WriteLine($"{diagnostic.Type}: {diagnostic.Message} ({diagnostic.Span.Start})");
                    }
                }
                else
                {
                    var generator = new CodeGenerator();
                    var func = generator.Generate(expression);

                    var result = func();

                    Console.WriteLine(result);
                }
            }
        }
    }
}
