using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.Utilites;
using System;
using Xunit;

namespace ExpressionEvaluator.Tests
{
	public class LexerTest
	{
		[Fact]
		public void ReadToken()
		{
			using (var input = StringHelpers.TextReaderFromString("2+3*6"))
			{
				var lexer = new Lexer(input);
				var token = lexer.ReadToken();
			}
		}

		[Fact]
		public void PeekToken()
		{
			using (var input = StringHelpers.TextReaderFromString("2+3*6"))
			{
				var lexer = new Lexer(input);
				var token = lexer.PeekToken();
			}
		}
	}
}
