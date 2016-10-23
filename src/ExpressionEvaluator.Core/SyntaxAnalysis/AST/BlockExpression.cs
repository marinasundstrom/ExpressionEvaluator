using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class BlockExpression : Expression, IEnumerable<Expression>
    {
        private List<Expression> _items = new List<Expression>();

        public void Add(Expression expr)
        {
            _items.Add(expr);
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
