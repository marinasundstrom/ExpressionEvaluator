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

            TokenInfo token = PeekToken();

            if (token.Kind == TokenKind.Caret)
            {
                ReadToken();

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

            token = ReadToken();

            switch (token.Kind)
            {
                case TokenKind.Identifier:
                    expr = new IdentifierExpression(token);
                    break;

                case TokenKind.Number:
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
                    break;

                case TokenKind.OpenParen:
                    token2 = PeekToken();
                    if (token2.Kind != TokenKind.CloseParen)
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
                    expr = new ParenthesisExpression(token, expr, token2);
                    break;

                case TokenKind.EndOfFile:
                    Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, token.GetSpan());
                    break;
            }

            return expr;
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

        #endregion
    }
}
