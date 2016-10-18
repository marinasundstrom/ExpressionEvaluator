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
@"let f a b = 
    if a > 2 then
        b
    else
        a
    end
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
