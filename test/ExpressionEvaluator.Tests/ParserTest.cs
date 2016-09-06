using System;
using Xunit;

namespace ExpressionEvaluator.Tests
{
	public class ParserTest
	{
		[Fact]
		public void Precedence_MultiplicationBeforeAddition()
		{
			using (var input = Helpers.TextReaderFromString("2+3*6"))
			{
				var lexer = new Lexer(input);
				var parser = new Parser(lexer);

				var expr = parser.ParseExpression() as BinaryExpression;

				Assert.True(expr.Left is NumberExpression);
				Assert.True(expr.Right is BinaryExpression);
			}
		}

		[Fact]
		public void Precedence_ParenthesisExpressionBeforeMultiplication()
		{
			using (var input = Helpers.TextReaderFromString("(2+3)*2"))
			{
				var lexer = new Lexer(input);
				var parser = new Parser(lexer);

				var expr = parser.ParseExpression() as BinaryExpression;

				Assert.True(expr.Left is ParenthesisExpression);
				Assert.True(expr.Right is NumberExpression);
			}
		}
	}
}
