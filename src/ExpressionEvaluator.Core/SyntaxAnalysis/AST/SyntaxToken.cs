using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public struct SyntaxToken
    {
        public readonly static SyntaxToken Empty = new SyntaxToken();

        public readonly static SyntaxToken Missing = new SyntaxToken(SyntaxKind.Missing);

        private IEnumerable<SyntaxTrivia> leadingTrivia;
        private IEnumerable<SyntaxTrivia> trailingTrivia;

        public SyntaxToken(SyntaxKind kind)
        {
            Kind = kind;
            Width = 0;
            Value = String.Empty;

            leadingTrivia = null;
            trailingTrivia = null;
        }

        public SyntaxKind Kind { get; }  
        public int Width { get; }
        public string Value { get; }

        public IEnumerable<SyntaxTrivia> LeadingTrivia
        {
            get
            {
                return leadingTrivia;
            }
        }

        public IEnumerable<SyntaxTrivia> TrailingTrivia
        {
            get
            {
                return trailingTrivia;
            }
        }

        public SourceSpan GetSpan()
        {
            return new SourceSpan();
        }

        public SourceSpan GetFullSpan()
        {
            return new SourceSpan();
        }

        public SyntaxToken With(IEnumerable<SyntaxTrivia> leading, IEnumerable<SyntaxTrivia> trailing)
        {
            leadingTrivia = leading;
            trailingTrivia = trailing;

            return this;
        }
    }
}
