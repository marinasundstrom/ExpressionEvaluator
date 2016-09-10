using ExpressionEvaluator.CodeGen;
using ExpressionEvaluator.LexicalAnalysis;
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
@"let x = 2
if x > 2 then
    x
end
";
            using (var reader = StringHelpers.TextReaderFromString(input))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expression = parser.ParseExpression();

                if (parser.Diagnostics.Any())
                {
                    foreach (var diagnostic in parser.Diagnostics)
                    {
                        Console.WriteLine($"{diagnostic.Type}: {diagnostic.Message} ({diagnostic.Span.Start})");
                    }

                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
}
