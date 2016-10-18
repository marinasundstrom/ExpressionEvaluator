using ExpressionEvaluator.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class ParenthesisExpression : Expression
    {
        public ParenthesisExpression(SyntaxToken openParen, Expression expression, SyntaxToken closeParen)
        {
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }

        public SyntaxToken OpenParen { get; private set; }

        public Expression Expression { get; private set; }

        public SyntaxToken CloseParen { get; private set; }
    }
}
