using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.Utilites;
using System;
using Xunit;

namespace ExpressionEvaluator.Tests
{
	public class LexerTest
	{
        [Fact(DisplayName = nameof(ReadPeekToken))]
        public void ReadPeekToken()
        {
            using (var reader = StringHelpers.TextReaderFromString("42+3"))
            {
                var lexer = new Lexer(reader);

                var token1 = lexer.PeekToken();
                var token2 = lexer.ReadToken();

                Assert.Equal(TokenKind.Number, token2.Kind);
                Assert.Equal(token1.Kind, token2.Kind);

                var token3 = lexer.ReadToken();

                Assert.Equal(TokenKind.Plus, token3.Kind);

                var token4 = lexer.PeekToken();
                var token5 = lexer.ReadToken();

                Assert.Equal(TokenKind.Number, token4.Kind);
                Assert.Equal(token1.Kind, token5.Kind);
            }
        }

        [Fact(DisplayName = nameof(ReadTokenCore))]
        public void ReadTokenCore()
        {
            using (var reader = StringHelpers.TextReaderFromString("42+3*6"))
            {
                var lexer = new Lexer(reader);

                var token1 = lexer.ReadTokenCore();
                var token2 = lexer.ReadTokenCore();
                var token3 = lexer.ReadTokenCore();
                var token4 = lexer.ReadTokenCore();
                var token5 = lexer.ReadTokenCore();

                Assert.Equal(TokenKind.Number, token1.Kind);
                Assert.Equal(1, token1.Column);

                Assert.Equal(TokenKind.Plus, token2.Kind);
                Assert.Equal(3, token2.Column);

                Assert.Equal(TokenKind.Number, token3.Kind);
                Assert.Equal(4, token3.Column);

                Assert.Equal(TokenKind.Star, token4.Kind);
                Assert.Equal(5, token4.Column);

                Assert.Equal(TokenKind.Number, token5.Kind);
                Assert.Equal(6, token5.Column);
            }
        }

        [Fact(DisplayName = nameof(ReadTokenCore_WithSpaces))]
        public void ReadTokenCore_WithSpaces()
        {
            using (var reader = StringHelpers.TextReaderFromString("42 + 3 * 6"))
            {
                var lexer = new Lexer(reader);

                var token1 = lexer.ReadTokenCore();
                var token2 = lexer.ReadTokenCore();
                var token3 = lexer.ReadTokenCore();
                var token4 = lexer.ReadTokenCore();
                var token5 = lexer.ReadTokenCore();

                Assert.Equal(TokenKind.Number, token1.Kind);
                Assert.Equal(1, token1.Column);

                Assert.Equal(TokenKind.Plus, token2.Kind);
                Assert.Equal(4, token2.Column);

                Assert.Equal(TokenKind.Number, token3.Kind);
                Assert.Equal(6, token3.Column);

                Assert.Equal(TokenKind.Star, token4.Kind);
                Assert.Equal(8, token4.Column);

                Assert.Equal(TokenKind.Number, token5.Kind);
                Assert.Equal(10, token5.Column);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Number_OneDigit))]
        public void ReadToken_Number_OneDigit()
        {
            TokenKind expectedValueKind = TokenKind.Number;
            string expectedValueString = "5";

            string inputString = "5";

            using (var reader = StringHelpers.TextReaderFromString(inputString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Number_MoreThanOneDigit))]
        public void ReadToken_Number_MoreThanOneDigit()
        {
            TokenKind expectedValueKind = TokenKind.Number;
            string expectedValueString = "42";

            string inputString = "42";

            using (var reader = StringHelpers.TextReaderFromString(inputString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Number_WithTrailingSpace))]
        public void ReadToken_Number_WithTrailingSpace()
        {
            TokenKind expectedValueKind = TokenKind.Number;
            string expectedValueString = "6";

            string inputString = "6 ";

            using (var reader = StringHelpers.TextReaderFromString(inputString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Plus))]
        public void ReadToken_Plus()
        {
            TokenKind expectedValueKind = TokenKind.Plus;
            string expectedValueString = "+";

            using (var reader = StringHelpers.TextReaderFromString(expectedValueString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Minus))]
        public void ReadToken_Minus()
        {
            TokenKind expectedValueKind = TokenKind.Minus;
            string expectedValueString = "-";

            using (var reader = StringHelpers.TextReaderFromString(expectedValueString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Star))]
        public void ReadToken_Star()
        {
            TokenKind expectedValueKind = TokenKind.Star;
            string expectedValueString = "*";

            using (var reader = StringHelpers.TextReaderFromString(expectedValueString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Slash))]
        public void ReadToken_Slash()
        {
            TokenKind expectedValueKind = TokenKind.Slash;
            string expectedValueString = "/";

            using (var reader = StringHelpers.TextReaderFromString(expectedValueString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_IfKeyword))]
        public void ReadToken_IfKeyword()
        {
            TokenKind expectedValueKind = TokenKind.IfKeyword;
            string expectedValueString = "if";

            using (var reader = StringHelpers.TextReaderFromString(expectedValueString))
            {
                var lexer = new Lexer(reader);
                var token = lexer.ReadTokenCore();

                Assert.Equal(expectedValueKind, token.Kind);
                Assert.Equal(expectedValueString, token.Value);
            }
        }
    }
}
