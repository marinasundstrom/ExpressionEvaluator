using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using ExpressionEvaluator.Utilites;
using System;
using Xunit;

namespace ExpressionEvaluator.Tests
{
	public class ParserTest
	{
        [Fact(DisplayName = nameof(ReadToken2))]
        public void ReadToken2()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var x = parser.ReadToken();
            }
        }

        [Fact(DisplayName = nameof(MaybeEat_Success))]
        public void MaybeEat_Success()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                SyntaxToken token, token2;

                var result = parser.MaybeEat(SyntaxKind.Number, out token);

                Assert.True(result);

                Assert.NotEqual(SyntaxKind.Missing, token.Kind);
            }
        }

        [Fact(DisplayName = nameof(MaybeEat_Fail))]
        public void MaybeEat_Fail()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                SyntaxToken token, token2;

                var result = parser.MaybeEat(SyntaxKind.Identifier, out token);

                Assert.False(result);

                Assert.Equal(SyntaxKind.Missing, token.Kind);

                token2 = parser.PeekToken();

                Assert.Equal(SyntaxKind.Number, token2.Kind);
            }
        }

        [Fact(DisplayName = nameof(Eat_Success))]
        public void Eat_Success()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                SyntaxToken token, token2;

                var result = parser.Eat(SyntaxKind.Number, out token);

                Assert.True(result);

                token2 = parser.PeekToken();

                Assert.NotEqual(token2.Kind, token.Kind);
            }
        }

        [Fact(DisplayName = nameof(Eat_Fail))]
        public void Eat_Fail()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                SyntaxToken token, token2;

                var result = parser.Eat(SyntaxKind.Identifier, out token);

                Assert.False(result);

                token2 = parser.PeekToken();

                Assert.NotEqual(token2.Kind, token.Kind);
            }
        }

        [Fact(DisplayName = nameof(IntNumberExpression))]
        public void IntNumberExpression()
		{
			using (var reader = StringHelpers.TextReaderFromString("2"))
			{
				var lexer = new Lexer(reader);
				var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as IntegerNumberExpression;

				Assert.True(expr is IntegerNumberExpression);
                Assert.NotEqual(SyntaxToken.Empty, expr.Token);
                Assert.Equal(2, expr.Value);
            }
		}

        [Fact(DisplayName = nameof(IntNumberExpression_TwoDigits))]
        public void IntNumberExpression_TwoDigits()
        {
            using (var reader = StringHelpers.TextReaderFromString("42"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as IntegerNumberExpression;

                Assert.True(expr is IntegerNumberExpression);
                Assert.NotEqual(SyntaxToken.Empty, expr.Token);
                Assert.Equal(42, expr.Value);
            }
        }

        [Fact(DisplayName = nameof(RealNumberExpression))]
        public void RealNumberExpression()
        {
            using (var reader = StringHelpers.TextReaderFromString("4.2"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as RealNumberExpression;

                Assert.True(expr is RealNumberExpression);
                Assert.NotEqual(SyntaxToken.Empty, expr.Number);
                Assert.NotEqual(SyntaxToken.Empty, expr.Separator);
                Assert.NotEqual(SyntaxToken.Empty, expr.Fraction);
                Assert.Equal(4.2, expr.Value);
            }
        }

        [Fact(DisplayName = nameof(BinaryExpression))]
        public void BinaryExpression()
        {
            using (var reader = StringHelpers.TextReaderFromString("4 + 3.2"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as BinaryExpression;

                Assert.True(expr is BinaryExpression);
                Assert.NotEqual(SyntaxToken.Empty, expr.Operator);
                Assert.True(expr.Left is NumberExpression);
                Assert.True(expr.Right is NumberExpression);
            }
        }

        [Fact(DisplayName = nameof(BinaryExpression_OperatorPrecedence))]
        public void BinaryExpression_OperatorPrecedence()
        {
            
        }

        [Fact(DisplayName = nameof(IfExpression))]
        public void IfExpression()
        {
            using (var reader = StringHelpers.TextReaderFromString("if x > 2 then 2 end"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as IfThenExpression;
            }
        }

        [Fact(DisplayName = nameof(LetExpression))]
        public void LetExpression()
        {
            using (var reader = StringHelpers.TextReaderFromString("let x = 2 > 3"))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression() as LetExpression;
            }
        }

        [Fact(DisplayName = nameof(Test))]
        public void Test()
        {
            var source = @"let x = (if true then 1 else 0)";

            using (var reader = StringHelpers.TextReaderFromString(source))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression();
            }
        }

        [Fact(DisplayName = nameof(Test2))]
        public void Test2()
        {
            var source = 
@"let x = 2
if x > 2 then
    x
end
";
            using (var reader = StringHelpers.TextReaderFromString(source))
            {
                var lexer = new Lexer(reader);
                var parser = new Parser(lexer);

                var expr = parser.ParseExpression();
                var exp2 = parser.ParseExpression();
            }
        }
    }
}
