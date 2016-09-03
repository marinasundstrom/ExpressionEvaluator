using System;

namespace ExpressionEvaluator
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			while (true)
			{
				Console.Write("Enter an expression > ");

				var line = Console.ReadLine();

				using (var input = Helpers.TextReaderFromString(line))
				{
					var lexer = new Lexer(input);
					var parser = new Parser(lexer);
					var expression = parser.ParseExpression();

					var result = Evaluator.EvaluateExpression(expression);

					Console.WriteLine(result);
				}

				Console.WriteLine();
			}
		}
	}
}
