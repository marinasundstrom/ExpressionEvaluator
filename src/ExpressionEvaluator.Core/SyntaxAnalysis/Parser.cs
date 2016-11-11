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
    public partial class Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ExpressionEvaluator.Parser"/> class.
        /// </summary>
        /// <param name="lexer">Lexer.</param>
        public Parser(Lexer lexer)
        {
            Lexer = lexer;

            Diagnostics = lexer.Diagnostics;

            Enumerator = new TokenEnumerator(lexer);
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

        public Expression Parse()
        {
            var block = new BlockExpression();
            while (!Lexer.IsEof)
            {
                var expr = ParseExpression();
                block.Add(expr);
            }
            return block;
        }

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
            SyntaxToken token;
            while (MaybeEat(SyntaxKind.Or, out token))
            {
                ret = new BinaryExpression(token, ret, ParseAndExpression());
            }
            return ret;
        }

        private Expression ParseAndExpression()
        {
            Expression ret = ParseNotExpression();
            SyntaxToken token;
            while (MaybeEat(SyntaxKind.And, out token))
            {
                ret = new BinaryExpression(token, ret, ParseAndExpression());
            }
            return ret;
        }

        private Expression ParseNotExpression()
        {
            var token = SyntaxToken.Empty;
            if (MaybeEat(SyntaxKind.NotKeyword, out token))
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
                    case SyntaxKind.CloseAngleBracket:
                    case SyntaxKind.GreaterOrEqual:
                    case SyntaxKind.Less:
                    case SyntaxKind.OpenAngleBracket:
                    case SyntaxKind.Equal:
                    case SyntaxKind.NotEquals:
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
        private bool TryResolveOperatorPrecedence(SyntaxToken candidateToken, out int precedence)
        {
            switch (candidateToken.Kind)
            {
                case SyntaxKind.Percent:
                case SyntaxKind.Slash:
                case SyntaxKind.Star:
                    precedence = 2;
                    break;

                case SyntaxKind.Minus:
                case SyntaxKind.Plus:
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

            SyntaxToken token = PeekToken();

            switch (token.Kind)
            {
                case SyntaxKind.Plus:
                    ReadToken();
                    expr = ParseFactorExpression();
                    expr = new UnaryExpression(token, expr);
                    break;

                case SyntaxKind.Minus:
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

            if (MaybeEat(SyntaxKind.Caret, out var token))
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

            SyntaxToken token;

            token = PeekToken();

            switch (token.Kind)
            {
                case SyntaxKind.Identifier:
                    ReadToken();
                    expr = new IdentifierExpression(token);
                    //while (MaybeEat(SyntaxKind.Period))
                    //{

                    //}
                    SyntaxToken openParen;
                    SyntaxToken closeParen;
                    if (MaybeEat(SyntaxKind.OpenParen, out openParen))
                    {
                        ArgumentList<Expression> argumentList = null;
                        var list = new List<Argument<Expression>>();

                        while(!MaybeEat(SyntaxKind.CloseParen, out closeParen))
                        {
                            var argExpr = ParseExpression();

                            SyntaxToken comma;
                            if(MaybeEat(SyntaxKind.Comma, out comma))
                            {
                                var t2 = PeekToken();
                                if(t2.Kind == SyntaxKind.CloseParen)
                                {
                                    Diagnostics.AddError(string.Format(Strings.Error_UnexpectedToken, comma), comma.GetSpan());
                                }
                            }

                            list.Add(new Argument<Expression>(argExpr, comma));
                        }
                        argumentList = new ArgumentList<Expression>(openParen, list, closeParen);
                        expr = new MethodInvokeExpression((IdentifierExpression)expr, argumentList);
                    }
                    break;

                case SyntaxKind.TrueKeyword:
                    ReadToken();
                    expr = new TrueLiteralExpression(token);
                    break;

                case SyntaxKind.FalseKeyword:
                    ReadToken();
                    expr = new FalseLiteralExpression(token);
                    break;

                case SyntaxKind.IfKeyword:
                    expr = ParseIfThenElseExpression();
                    break;

                case SyntaxKind.LetKeyword:
                    expr = ParseLetExpression();
                    break;

                case SyntaxKind.Number:
                    expr = ParseNumberExpression();
                    break;

                case SyntaxKind.OpenParen:
                    expr = ParseParenthesisExpression();
                    break;

                case SyntaxKind.EndOfFile:
                    ReadToken();
                    Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, token.GetSpan());
                    break;
            }

            return expr;
        }

        private Expression ParseParenthesisExpression()
        {
            SyntaxToken token, token2;
            Expression expr = null;

            token = ReadToken();
            token2 = PeekToken();
            if (!MaybeEat(SyntaxKind.CloseParen, out token2))
            {
                expr = ParseExpression();
            }
            if (expr == null)
            {
                Diagnostics.AddError(string.Format(Strings.Error_InvalidExpressionTerm, token2.Value), token2.GetSpan());
            }
            else
            {
                if (!Eat(SyntaxKind.CloseParen, out token2))
                {
                    Diagnostics.AddError(string.Format(Strings.Error_ExpectedToken, ')'), token2.GetSpan());
                }
            }
            return new ParenthesisExpression(token, expr, token2);
        }

        private Expression ParseNumberExpression()
        {
            SyntaxToken token, token2, token3;
            Expression expr = null;

            token = ReadToken();
            if (MaybeEat(SyntaxKind.Period, out token2))
            {
                if (MaybeEat(SyntaxKind.Number, out token3))
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
            SyntaxToken nameToken, assignToken;

            List<Parameter> parameters = new List<Parameter>();
            
            Expression assignedExpression = null;
            if (!Eat(SyntaxKind.Identifier, out nameToken))
            {
                Diagnostics.AddError(Strings.Error_UnexpectedEndOfFile, nameToken.GetSpan());
            }
            SyntaxToken token = PeekToken();
            if(token.Kind != SyntaxKind.Assign && token.Kind == SyntaxKind.Identifier)
            {
                while(MaybeEat(SyntaxKind.Identifier, out token))
                {
                    parameters.Add(new Parameter(token));
                }
            }

            if (!Eat(SyntaxKind.Assign, out assignToken))
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
            SyntaxToken token, token2, token3;
            token = ReadToken();
            var condition = ParseExpression();
            SyntaxToken thenKeyword, endKeyword;
            Expression body = null;
            Expression elseBody = null;
            if (!Eat(SyntaxKind.ThenKeyword, out thenKeyword))
            {
                Diagnostics.AddError(string.Format(Strings.Error_ExpectedKeyword, "then"), thenKeyword.GetSpan());
            }

            if (!MaybeEat(SyntaxKind.EndKeyword, out token2))
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
            if (MaybeEat(SyntaxKind.ElseKeyword, out token3))
            {
                elseBody = ParseExpression();
                if (elseBody == null)
                {
                    Diagnostics.AddError(Strings.Error_ExpectedExpression, token3.GetSpan());
                }
            }
            MaybeEat(SyntaxKind.EndKeyword, out endKeyword);
            return new IfThenExpression(token, condition, thenKeyword, body, token3, elseBody, endKeyword);
        }

        #region Lexer helpers

        internal SyntaxToken PeekToken()
        {
            return Enumerator.PeekToken();
        }

        internal SyntaxToken ReadToken()
        {
            return Enumerator.ReadToken();
        }

        internal bool MaybeEat(SyntaxKind kind)
        {
            SyntaxToken syntaxToken;
            return MaybeEat(kind, out syntaxToken);
        }

        internal bool MaybeEat(SyntaxKind kind, out SyntaxToken syntaxToken)
        {
            syntaxToken = PeekToken();
            if (syntaxToken.Kind == kind)
            {
                ReadToken();
                return true;
            }
            syntaxToken = SyntaxToken.Missing;
            return false;
        }

        internal bool Eat(SyntaxKind kind)
        {
            SyntaxToken syntaxToken;
            return Eat(kind, out syntaxToken);
        }

        internal bool Eat(SyntaxKind kind, out SyntaxToken syntaxToken)
        {
            syntaxToken = ReadToken();
            if (syntaxToken.Kind == kind)
            {
                return true;
            }
            syntaxToken = SyntaxToken.Missing;
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

        private TokenEnumerator Enumerator { get; set; }

        #endregion
    }
}
