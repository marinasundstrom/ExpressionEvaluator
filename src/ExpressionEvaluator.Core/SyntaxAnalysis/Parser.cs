using ExpressionEvaluator.Diagnostics;
using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.Properties;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="precedence">The current level of precedence.</param>
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

            TokenInfo token;

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
                    expr = ParseIfThenElseExpression();
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
            TokenInfo token, token2;
            Expression expr = null;

            token = ReadToken();
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
            TokenInfo token, token2, token3;
            Expression expr = null;

            token = ReadToken();
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
            TokenInfo nameToken, assignToken;

            List<Parameter> parameters = new List<Parameter>();
            
            Expression assignedExpression = null;
            if (!Eat(TokenKind.Identifier, out nameToken))
            {
                Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, nameToken.GetSpan());
            }
            TokenInfo token = PeekToken();
            if(token.Kind != TokenKind.Assign && token.Kind == TokenKind.Identifier)
            {
                while(MaybeEat(TokenKind.Identifier, out token))
                {
                    parameters.Add(new Parameter(token));
                }
            }
            else
            {
                ReadToken();
                Diagnostics.AddError(Strings.Error_ExpectedToken, token.GetSpan());
            }
            if (!Eat(TokenKind.Assign, out assignToken))
            {
                Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, assignToken.GetSpan());
            }
            token = PeekToken();
            assignedExpression = ParseExpression();
            if(assignedExpression == null)
            {
                Diagnostics.AddError(Strings.Error_ExpectedExpression, token.GetSpan());
            }
            var expr = new LetExpression(letKeyword, nameToken, assignToken, assignedExpression);
            if(parameters.Any())
            {
                expr.Parameters.AddRange(parameters);
            }
            return expr;
        }

        private Expression ParseIfThenElseExpression()
        {
            TokenInfo token, token2, token3;
            token = ReadToken();
            var condition = ParseExpression();
            TokenInfo thenKeyword, endKeyword;
            Expression body = null;
            Expression elseBody = null;
            if (!Eat(TokenKind.ThenKeyword, out thenKeyword))
            {
                Diagnostics.AddError(string.Format(Strings.Error_ExpectedKeyword, "then"), thenKeyword.GetSpan());
            }

            if (!MaybeEat(TokenKind.EndKeyword, out token2))
            {
                body = ParseExpression();
                if (body == null)
                {
                    Diagnostics.AddError(Strings.Error_ExpectedExpression, token2.GetSpan());
                }
            }
            else
            {
                Diagnostics.AddError(Strings.Error_ExpectedExpression, token2.GetSpan());
            }
            if (MaybeEat(TokenKind.ElseKeyword, out token3))
            {
                elseBody = ParseExpression();
                if (elseBody == null)
                {
                    Diagnostics.AddError(Strings.Error_ExpectedExpression, token3.GetSpan());
                }
            }
            MaybeEat(TokenKind.EndKeyword, out endKeyword);
            return new IfThenExpression(token, condition, thenKeyword, body, token3, elseBody, endKeyword);
        }

        #region Lexer helpers

        internal TokenInfo PeekToken()
        {
            return Lexer.PeekToken();
        }

        internal TokenInfo ReadToken()
        {
            return Lexer.ReadToken();
        }

        internal bool MaybeEat(TokenKind kind)
        {
            TokenInfo tokenInfo;
            return MaybeEat(kind, out tokenInfo);
        }

        internal bool MaybeEat(TokenKind kind, out TokenInfo tokenInfo)
        {
            tokenInfo = PeekToken();
            if (tokenInfo.Kind == kind)
            {
                ReadToken();
                return true;
            }
            return false;
        }

        internal bool Eat(TokenKind kind)
        {
            TokenInfo tokenInfo;
            return Eat(kind, out tokenInfo);
        }

        internal bool Eat(TokenKind kind, out TokenInfo tokenInfo)
        {
            tokenInfo = ReadToken();
            if (tokenInfo.Kind == kind)
            {
                return true;
            }
            return false;
        }

        internal bool IsEof
        {
            get
            {
                return Lexer.IsEof;
            }
        }

        internal bool IsEol
        {
            get
            {
                return Lexer.IsEol;
            }
        }

        internal int Indentation
        {
            get
            {
                return Lexer.Indentation;
            }
        }

        #endregion
    }
}
