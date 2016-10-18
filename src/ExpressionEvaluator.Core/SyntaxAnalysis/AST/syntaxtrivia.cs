using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public struct SyntaxTrivia
    {
        public SyntaxTrivia(SyntaxKind kind)
        {
            Kind = kind;
            Width = 0;
            Token = default(SyntaxToken);
        }

        public SyntaxKind Kind { get; }

        public int Width { get; }
               
        public SyntaxToken Token { get; }
    }
}
