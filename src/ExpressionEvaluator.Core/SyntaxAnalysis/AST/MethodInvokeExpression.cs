using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class MethodInvokeExpression : Expression
    {
        public MethodInvokeExpression(IdentifierExpression memberName, ArgumentList<Expression> arguments)
        {
            MemberName = memberName;
            Arguments = arguments;
        }

        public IdentifierExpression MemberName { get; }

        public ArgumentList<Expression> Arguments { get; }
    }
}
