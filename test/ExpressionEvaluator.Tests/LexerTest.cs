using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
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

                Assert.Equal(SyntaxKind.Number, token2.Kind);
                Assert.Equal(token1.Kind, token2.Kind);

                var token3 = lexer.ReadToken();

                Assert.Equal(SyntaxKind.Plus, token3.Kind);

                var token4 = lexer.PeekToken();
                var token5 = lexer.ReadToken();

                Assert.Equal(SyntaxKind.Number, token4.Kind);
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

                Assert.Equal(SyntaxKind.Number, token1.Kind);
                Assert.Equal(1, token1.Column);

                Assert.Equal(SyntaxKind.Plus, token2.Kind);
                Assert.Equal(3, token2.Column);

                Assert.Equal(SyntaxKind.Number, token3.Kind);
                Assert.Equal(4, token3.Column);

                Assert.Equal(SyntaxKind.Star, token4.Kind);
                Assert.Equal(5, token4.Column);

                Assert.Equal(SyntaxKind.Number, token5.Kind);
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

                Assert.Equal(SyntaxKind.Number, token1.Kind);
                Assert.Equal(1, token1.Column);

                Assert.Equal(SyntaxKind.Plus, token2.Kind);
                Assert.Equal(4, token2.Column);

                Assert.Equal(SyntaxKind.Number, token3.Kind);
                Assert.Equal(6, token3.Column);

                Assert.Equal(SyntaxKind.Star, token4.Kind);
                Assert.Equal(8, token4.Column);

                Assert.Equal(SyntaxKind.Number, token5.Kind);
                Assert.Equal(10, token5.Column);
            }
        }

        [Fact(DisplayName = nameof(ReadPeekToken_Newline))]
        public void ReadPeekToken_Newline()
        {
            using (var reader = StringHelpers.TextReaderFromString("42\r\n64 hej\r\n"))
            {
                var lexer = new Lexer(reader);

                Assert.False(lexer.IsEol);

                var token1 = lexer.PeekToken();
                var token2 = lexer.ReadToken();

                Assert.True(lexer.IsEol);

                var token3 = lexer.PeekToken();
                var token4 = lexer.ReadToken();

                Assert.False(lexer.IsEol);

                var token5 = lexer.PeekToken();
                var token6 = lexer.ReadToken();

                Assert.True(lexer.IsEol);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Newline))]
        public void ReadToken_Newline()
        {
            using (var reader = StringHelpers.TextReaderFromString("42\r\n64 hej\r\n"))
            {
                var lexer = new Lexer(reader);

                Assert.False(lexer.IsEol);

                var token2 = lexer.ReadToken();

                Assert.True(lexer.IsEol);

                var token4 = lexer.ReadToken();

                Assert.False(lexer.IsEol);

                var token6 = lexer.ReadToken();

                Assert.True(lexer.IsEol);
            }
        }

        [Fact(DisplayName = nameof(ReadToken_Number_OneDigit))]
        public void ReadToken_Number_OneDigit()
        {
            SyntaxKind expectedValueKind = SyntaxKind.Number;
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
            SyntaxKind expectedValueKind = SyntaxKind.Number;
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
            SyntaxKind expectedValueKind = SyntaxKind.Number;
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
            SyntaxKind expectedValueKind = SyntaxKind.Plus;
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
            SyntaxKind expectedValueKind = SyntaxKind.Minus;
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
            SyntaxKind expectedValueKind = SyntaxKind.Star;
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
            SyntaxKind expectedValueKind = SyntaxKind.Slash;
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
            SyntaxKind expectedValueKind = SyntaxKind.IfKeyword;
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
