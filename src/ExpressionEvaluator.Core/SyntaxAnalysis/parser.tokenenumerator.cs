using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis
{
    partial class Parser
    {
        class TokenEnumerator
        {
            private IEnumerator<SyntaxToken> enumerator;
            private TokenState? state;

            public TokenEnumerator(Lexer lexer)
            {
                Lexer = lexer;
            }

            public Lexer Lexer { get; }

            public SyntaxToken PeekToken()
            {
                if (state != null)
                {
                    var s = (TokenState)state;
                    var peekedToken = s.Token;

                    return peekedToken;
                }
                else
                {
                    var peekToken = ReadTokenCore();
                    state = new TokenState(peekToken);
                    return peekToken;
                }
            }

            public SyntaxToken ReadToken()
            {
                if (state != null)
                {
                    var token = state.Value.Token;
                    state = null;
                    return token;
                }
                return ReadTokenCore();
            }

            private SyntaxToken ReadTokenCore()
            {
                if (enumerator == null)
                {
                    enumerator = ReadTokenCore2().GetEnumerator();
                }

                enumerator.MoveNext();
                return enumerator.Current;
            }

            private IEnumerable<SyntaxToken> ReadTokenCore2()
            {
                SyntaxToken resultToken;

                while (!Lexer.IsEof)
                {
                    var leadingTrivia = ReadTrivia().ToArray();

                    var token = Lexer.ReadToken();

                    switch (token.Kind)
                    {
                        case SyntaxKind.Identifier:
                            resultToken = new SyntaxToken(SyntaxKind.Identifier);
                            break;

                        case SyntaxKind.Number:
                            resultToken = new SyntaxToken(SyntaxKind.Number);
                            break;

                        default:
                            resultToken = new SyntaxToken(token.Kind);
                            break;
                    }

                    var trailingTrivia = ReadTrivia().ToArray();

                    yield return resultToken.With(leadingTrivia, trailingTrivia);
                }
            }

            private IEnumerable<SyntaxTrivia> ReadTrivia()
            {
                while (!Lexer.IsEof)
                {
                    var token = Lexer.PeekToken();

                    switch (token.Kind)
                    {
                        case SyntaxKind.Whitespace:
                            Lexer.ReadToken();
                            yield return new SyntaxTrivia(SyntaxKind.Whitespace);
                            break;

                        case SyntaxKind.Tab:
                            Lexer.ReadToken();
                            yield return new SyntaxTrivia(SyntaxKind.Tab);
                            break;

                        case SyntaxKind.CarriageReturn:
                            Lexer.ReadToken();
                            yield return new SyntaxTrivia(SyntaxKind.CarriageReturn);
                            break;

                        case SyntaxKind.Newline:
                            Lexer.ReadToken();
                            yield return new SyntaxTrivia(SyntaxKind.Newline);
                            break;

                        default:
                            yield break;
                    }
                }
            }

            /// <summary>
            /// Encapsulates a lookahead state.
            /// </summary>
            struct TokenState
            {
                public TokenState(SyntaxToken token)
                {
                    Token = token;
                }

                public SyntaxToken Token { get; }
            }
        }
    }
}
