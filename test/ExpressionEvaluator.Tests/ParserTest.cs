using System;
using Xunit;

namespace ExpressionEvaluator.Tests
{
	public class ParserTest
	{
		[Fact]
		public void MultiplicationBeforeAddition()
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
		public void ParenthesisExpression()
		{
			using (var input = Helpers.TextReaderFromString("(2+3)*2"))
			{
				var lexer = new Lexer(input);
				var parser = new Parser(lexer);

				var expr = parser.ParseExpression() as BinaryExpression;

				Assert.True(expr.Left is BinaryExpression);
				Assert.True(expr.Right is NumberExpression);
			}
		}
	}
}
