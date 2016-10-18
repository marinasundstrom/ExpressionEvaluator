using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class SyntaxNode
    {
        public SyntaxKind Kind { get; }

        public int Width { get; }
    }
}
