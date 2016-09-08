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
            var token = Lexer.PeekToken();
            while (token.Kind == TokenKind.Or)
            {
                Lexer.ReadToken();
                ret = new BinaryExpression(token, ret, ParseAndExpression());

                token = Lexer.PeekToken();
            }
            return ret;
        }

        private Expression ParseAndExpression()
        {
            Expression ret = ParseNotExpression();
            var token = Lexer.PeekToken();
            while (token.Kind == TokenKind.And)
            {
                Lexer.ReadToken();
                ret = new BinaryExpression(token, ret, ParseAndExpression());

                token = Lexer.PeekToken();
            }
            return ret;
        }

        private Expression ParseNotExpression()
        {
            var token = Lexer.PeekToken();
            if (token.Kind == TokenKind.NotKeyword)
            {
                Lexer.ReadToken();
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
                var token = Lexer.PeekToken();

                switch (token.Kind)
                {
                    case TokenKind.CloseAngleBracket:
                    case TokenKind.GreaterOrEqual:
                    case TokenKind.Less:
                    case TokenKind.OpenAngleBracket:
                        Lexer.ReadToken();
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
                var operatorCandidate = Lexer.PeekToken();

                int prec;
                if (!TryResolveOperatorPrecedence(operatorCandidate, out prec))
                    return expr;
                    
                Lexer.ReadToken();

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

        /// <summar>y
        /// Parse a factor expression.
        /// </summary>
        /// <returns>An expression.</returns>
        private Expression ParseFactorExpression()
        {
            Expression expr;

            TokenInfo token = Lexer.PeekToken();

            switch (token.Kind)
            {
                case TokenKind.Plus:
                    expr = ParseFactorExpression();
                    expr = new UnaryExpression(token, expr);
                    break;

                case TokenKind.Minus:
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

            TokenInfo token = Lexer.PeekToken();

            if (token.Kind == TokenKind.Caret)
            {
                Lexer.ReadToken();

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

            token = Lexer.ReadToken();

            switch (token.Kind)
            {
                case TokenKind.OpenParen:
                    expr = ParseExpression();
                    token2 = Lexer.ReadToken();
                    if (token2.Kind != TokenKind.CloseParen)
                    {
                        Diagnostics.AddError(string.Format(Strings.Error_ExpectedToken, ')'), token.GetSpan());
                    }
                    expr = new ParenthesisExpression(token, expr, token2);
                    break;

                case TokenKind.Number:
                    token2 = Lexer.PeekToken();
                    if (token2.Kind == TokenKind.Period)
                    {
                        Lexer.ReadToken();

                        token3 = Lexer.PeekToken();
                        if (token3.Kind == TokenKind.Number)
                        {
                            Lexer.ReadToken();

                            expr = new RealNumberExpression(token, token2, token3);
                        }
                        else
                        {
                            Diagnostics.AddError(string.Format(Strings.Error_UnexpectedToken, token3.Value), token.GetSpan());
                        }
                    }
                    else
                    {
                        expr = new IntegerNumberExpression(token);
                    }
                    break;

                case TokenKind.Identifier:
                    expr = new IdentifierExpression(token);
                    break;

                case TokenKind.EndOfFile:
                    Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, token.GetSpan());
                    break;

                default:
                    if(token.Kind != TokenKind.Invalid) {
                        Diagnostics.AddError(Strings.Error_UnexpectedToken, token.GetSpan());
                    }
                    break;
            }

            return expr;
        }
    }
}
