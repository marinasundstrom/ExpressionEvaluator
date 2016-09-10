using ExpressionEvaluator.Diagnostics;
using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.Properties;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
namespace ExpressionEvaluator.SyntaxAnalysis
{
    /// <summary>
    /// Parser.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ExpressionEvaluator.Parser"/> class.
        /// </summary>
        /// <param name="lexer">Lexer.</param>
        public Parser(Lexer lexer)
        {
            Lexer = lexer;

            Diagnostics = lexer.Diagnostics;
        }

        /// <summary>
        /// Gets the lexer.
        /// </summary>
        /// <value>The lexer.</value>
        public Lexer Lexer { get; }

        /// <summary>
        /// Gets the diagnostics bag.
        /// </summary>
        /// <value>The diagnostics bag.</value>
        public DiagnosticsBag Diagnostics { get; }

        /// <summary>
        /// Parse an expression.
        /// </summary>
        /// <returns>An expression.</returns>
        public Expression ParseExpression()
        {
            return ParseOrExpression();
        }

        private Expression ParseOrExpression()
        {
            Expression ret = ParseAndExpression();
            TokenInfo token;
            while (MaybeEat(TokenKind.Or, out token))
            {
                ret = new BinaryExpression(token, ret, ParseAndExpression());
            }
            return ret;
        }

        private Expression ParseAndExpression()
        {
            Expression ret = ParseNotExpression();
            TokenInfo token;
            while (MaybeEat(TokenKind.And, out token))
            {
                ret = new BinaryExpression(token, ret, ParseAndExpression());
            }
            return ret;
        }

        private Expression ParseNotExpression()
        {
            var token = TokenInfo.Empty;
            if (MaybeEat(TokenKind.NotKeyword, out token))
            {
                Expression ret = new UnaryExpression(token, ParseNotExpression());
                return ret;
            }
            else
            {
                return ParseComparisonExpression();
            }
        }

        /// <summary>
        /// Parse a comparison expression.
        /// </summary>
        /// <returns>An expression.</returns>
        internal Expression ParseComparisonExpression()
        {
            Expression expr = ParseExpressionCore(0);
            while (true)
            {
                var token = PeekToken();

                switch (token.Kind)
                {
                    case TokenKind.CloseAngleBracket:
                    case TokenKind.GreaterOrEqual:
                    case TokenKind.Less:
                    case TokenKind.OpenAngleBracket:
                    case TokenKind.Equal:
                    case TokenKind.NotEquals:
                        ReadToken();
                        break;
                    default:
                        return expr;
                }
                Expression rhs = ParseComparisonExpression();
                expr = new BinaryExpression(token, expr, rhs);
            }
        }

        /// <summary>
        /// Parse an expression (Internal)
        /// </summary>
        /// <returns>An expression.</returns>
        /// <param name="precedence">The current level of precendence.</param>
        private Expression ParseExpressionCore(int precedence)
        {
            var expr = ParseFactorExpression();

            while (true)
            {
                var operatorCandidate = PeekToken();

                int prec;
                if (!TryResolveOperatorPrecedence(operatorCandidate, out prec))
                    return expr;

                ReadToken();

                if (prec >= precedence)
                {
                    var right = ParseExpressionCore(prec + 1);
                    expr = new BinaryExpression(operatorCandidate, expr, right);
                }
                else
                {
                    return expr;
                }
            }
        }

        /// <summary>
        /// Try to resolve an operation from a specified candidate token.
        /// </summary>
        /// <returns><c>true</c>, if the token is an operation, <c>false</c> otherwise.</returns>
        /// <param name="candidateToken">The candidate token for operation.</param>
        /// <param name="precedence">The operator precedence for the resolved operation.</param>
        private bool TryResolveOperatorPrecedence(TokenInfo candidateToken, out int precedence)
        {
            switch (candidateToken.Kind)
            {
                case TokenKind.Percent:
                case TokenKind.Slash:
                case TokenKind.Star:
                    precedence = 2;
                    break;

                case TokenKind.Minus:
                case TokenKind.Plus:
                    precedence = 1;
                    break;

                default:
                    precedence = -1;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Parse a factor expression.
        /// </summary>
        /// <returns>An expression.</returns>
        private Expression ParseFactorExpression()
        {
            Expression expr;

            TokenInfo token = PeekToken();

            switch (token.Kind)
            {
                case TokenKind.Plus:
                    ReadToken();
                    expr = ParseFactorExpression();
                    expr = new UnaryExpression(token, expr);
                    break;

                case TokenKind.Minus:
                    ReadToken();
                    expr = ParseFactorExpression();
                    expr = new UnaryExpression(token, expr);
                    break;

                default:
                    return ParsePowerExpression();
            }

            return expr;
        }

        /// <summary>
        /// Parse a power expression.
        /// </summary>
        /// <returns>An expression.</returns>
        private Expression ParsePowerExpression()
        {
            Expression expr = ParsePrimaryExpression();

            if (MaybeEat(TokenKind.Caret, out var token))
            {
                Expression right = ParseFactorExpression();
                expr = new BinaryExpression(token, expr, right);
            }

            return expr;
        }

        /// <summary>
        /// Parse a primary expression.
        /// </summary>
        /// <returns>An expression.</returns>
        private Expression ParsePrimaryExpression()
        {
            Expression expr = null;

            TokenInfo token, token2, token3;

            token = PeekToken();

            switch (token.Kind)
            {
                case TokenKind.Identifier:
                    ReadToken();
                    expr = new IdentifierExpression(token);
                    break;

                case TokenKind.TrueKeyword:
                    ReadToken();
                    expr = new TrueLiteralExpression(token);
                    break;

                case TokenKind.FalseKeyword:
                    ReadToken();
                    expr = new FalseLiteralExpression(token);
                    break;

                case TokenKind.IfKeyword:
                    expr = ParseIfExpression();
                    break;

                case TokenKind.LetKeyword:
                    expr = ParseLetExpression();
                    break;

                case TokenKind.Number:
                    expr = ParseNumberExpression();
                    break;

                case TokenKind.OpenParen:
                    expr = ParseParenthesisExpression();
                    break;

                case TokenKind.EndOfFile:
                    ReadToken();
                    Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, token.GetSpan());
                    break;
            }

            return expr;
        }

        private Expression ParseParenthesisExpression()
        {
            TokenInfo token = TokenInfo.Empty, token2;
            Expression expr = null;

            ReadToken();
            token2 = PeekToken();
            if (!MaybeEat(TokenKind.CloseParen, out token2))
            {
                expr = ParseExpression();
            }
            if (expr == null)
            {
                Diagnostics.AddError(string.Format(Strings.Error_InvalidExpressionTerm, token2.Value), token2.GetSpan());
            }
            else
            {
                if (!Eat(TokenKind.CloseParen, out token2))
                {
                    Diagnostics.AddError(string.Format(Strings.Error_ExpectedToken, ')'), token2.GetSpan());
                }
            }
            return new ParenthesisExpression(token, expr, token2);
        }

        private Expression ParseNumberExpression()
        {
            TokenInfo token = TokenInfo.Empty, token2, token3;
            Expression expr = null;

            ReadToken();
            if (MaybeEat(TokenKind.Period, out token2))
            {
                if (MaybeEat(TokenKind.Number, out token3))
                {
                    expr = new RealNumberExpression(token, token2, token3);
                }
                else
                {
                    Diagnostics.AddError(string.Format(Strings.Error_UnexpectedToken, token3.Value), token3.GetSpan());
                }
            }
            else
            {
                expr = new IntegerNumberExpression(token);
            }

            return expr;
        }

        private Expression ParseLetExpression()
        {
            var letKeyword = ReadToken();
            TokenInfo nameToken;
            TokenInfo assignToken;
            Expression assignedExpression = null;
            if (!Eat(TokenKind.Identifier, out nameToken))
            {
                Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, nameToken.GetSpan());
            }
            if (!Eat(TokenKind.Assign, out assignToken))
            {
                Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, assignToken.GetSpan());
            }
            TokenInfo token = PeekToken();
            assignedExpression = ParseExpression();
            if(assignedExpression == null)
            {
                Diagnostics.AddError(Strings.Error_ExpectedExpression, token.GetSpan());
            }
            return new LetExpression(letKeyword, nameToken, assignToken, assignedExpression);
        }

        private Expression ParseIfExpression()
        {
            TokenInfo token, token2;
            token = ReadToken();
            var condition = ParseExpression();
            TokenInfo thenKeyword, endKeyword;
            Expression body = null;
            if (!Eat(TokenKind.ThenKeyword, out thenKeyword))
            {
                Diagnostics.AddError(string.Format(Strings.Error_ExpectedKeyword, "then"), thenKeyword.GetSpan());
            }
            if (!MaybeEat(TokenKind.EndKeyword, out token2))
            {
                body = ParseExpression();
            }
            else
            {
                Diagnostics.AddError(Strings.Error_ExpectedExpression, token2.GetSpan());
            }
            MaybeEat(TokenKind.EndKeyword, out endKeyword);
            return new IfThenExpression(token, condition, thenKeyword, body, endKeyword);
        }

        #region Lexer helpers

        private TokenInfo PeekToken()
        {
            return Lexer.PeekToken();
        }

        private TokenInfo ReadToken()
        {
            return Lexer.ReadToken();
        }

        private bool MaybeEat(TokenKind kind)
        {
            TokenInfo tokenInfo;
            return MaybeEat(kind, out tokenInfo);
        }

        private bool MaybeEat(TokenKind kind, out TokenInfo tokenInfo)
        {
            tokenInfo = PeekToken();
            if (tokenInfo.Kind == kind)
            {
                ReadToken();
                return true;
            }
            return false;
        }

        private bool Eat(TokenKind kind)
        {
            TokenInfo tokenInfo;
            return Eat(kind, out tokenInfo);
        }

        private bool Eat(TokenKind kind, out TokenInfo tokenInfo)
        {
            tokenInfo = ReadToken();
            if (tokenInfo.Kind == kind)
            {
                return true;
            }
            return false;
        }

        private bool IsEol
        {
            get
            {
                return Lexer.IsEol;
            }
        }

        #endregion
    }
}
