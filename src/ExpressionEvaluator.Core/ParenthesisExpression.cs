using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator
{
    public class ParenthesisExpression : Expression
    {
        public ParenthesisExpression(TokenInfo openParen, Expression expression, TokenInfo closeParen)
        {
            OpenParen = openParen;
            Expression = expression;
            CloseParen = closeParen;
        }

        public TokenInfo OpenParen { get; private set; }

        public Expression Expression { get; private set; }

        public TokenInfo CloseParen { get; private set; }
    }
}
