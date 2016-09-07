using ExpressionEvaluator.Properties;
using System;
namespace ExpressionEvaluator
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

            Diagnostics = new DiagnosticsBag();
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
        /// Parses an expression.
        /// </summary>
        /// <returns>An expression.</returns>
        public Expression ParseExpression()
        {
            return ParseExpressionCore(0);
        }

        /// <summary>
        /// Parses an expression (Internal)
        /// </summary>
        /// <returns>An expression.</returns>
        /// <param name="precedence">The current level of precendence.</param>
        private Expression ParseExpressionCore(int precedence)
        {
            BinaryOperation operation;
            int prec;

            var expr = ParseFactorExpression();

            while (true)
            {
                var operatorCandidate = Lexer.PeekToken();

                if (!TryResolveOperation(operatorCandidate, out operation, out prec))
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
        /// Tries to resolve an operation from a specified candidate token.
        /// </summary>
        /// <returns><c>true</c>, if the token is an operation, <c>false</c> otherwise.</returns>
        /// <param name="candidateToken">The candidate token for operation.</param>
        /// <param name="operation">The operation corresponding to the candidate token.</param>
        /// <param name="precedence">The operator precedence for the resolved operation.</param>
        private bool TryResolveOperation(TokenInfo candidateToken, out BinaryOperation operation, out int precedence)
        {
            switch (candidateToken.Kind)
            {
                case TokenKind.Slash:
                    operation = BinaryOperation.Divide;
                    precedence = 6;
                    break;

                case TokenKind.Star:
                    operation = BinaryOperation.Multiply;
                    precedence = 6;
                    break;

                case TokenKind.Minus:
                    operation = BinaryOperation.Subtract;
                    precedence = 5;
                    break;

                case TokenKind.Plus:
                    operation = BinaryOperation.Add;
                    precedence = 5;
                    break;

                case TokenKind.Less:
                    operation = BinaryOperation.Less;
                    precedence = 4;
                    break;

                case TokenKind.LessOrEqual:
                    operation = BinaryOperation.LessOrEquals;
                    precedence = 4;
                    break;

                case TokenKind.Greater:
                    operation = BinaryOperation.Greater;
                    precedence = 4;
                    break;

                case TokenKind.GreaterOrEqual:
                    operation = BinaryOperation.GreaterOrEquals;
                    precedence = 4;
                    break;

                case TokenKind.Equal:
                    operation = BinaryOperation.Equal;
                    precedence = 3;
                    break;

                case TokenKind.NotEquals:
                    operation = BinaryOperation.NotEquals;
                    precedence = 3;
                    break;

                case TokenKind.And:
                    operation = BinaryOperation.And;
                    precedence = 2;
                    break;

                case TokenKind.Or:
                    operation = BinaryOperation.Or;
                    precedence = 1;
                    break;

                default:
                    operation = BinaryOperation.None;
                    precedence = -1;
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Parses a factor expression.
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
        /// Parses a power expression.
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
        /// Parses a primary expression.
        /// </summary>
        /// <returns>An expression.</returns>
        private Expression ParsePrimaryExpression()
        {
            Expression expr = null;

            var token = Lexer.ReadToken();

            switch (token.Kind)
            {
                case TokenKind.OpenParen:
                    expr = ParseExpression();
                    var token2 = Lexer.ReadToken();
                    if (token2.Kind != TokenKind.CloseParen)
                    {
                        Diagnostics.AddError(string.Format(Strings.Error_ExpectedToken, ')'), token.GetSpan());
                    }
                    expr = new ParenthesisExpression(token, expr, token2);
                    break;

                case TokenKind.Number:
                    expr = new NumberExpression(token);
                    break;

                case TokenKind.Identifier:
                    expr = new IdentifierExpression(token);
                    break;

                case TokenKind.EndOfFile:
                    Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, token.GetSpan());
                    break;

                default:
                    Diagnostics.AddError(Strings.Error_UnexpectedToken, token.GetSpan());
                    break;
            }

            return expr;
        }

        private void ParseParenthesisExpression(out Expression expr, out TokenInfo token)
        {
            expr = ParseExpression();
            token = Lexer.ReadToken();
            if (token.Kind != TokenKind.CloseParen)
                Diagnostics.AddError(string.Format(Strings.Error_ExpectedToken, ')'), token.GetSpan());
        }
    }
}
