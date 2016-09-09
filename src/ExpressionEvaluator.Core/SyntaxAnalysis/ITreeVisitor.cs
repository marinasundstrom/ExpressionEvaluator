using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.SyntaxAnalysis
{
    public interface ITreeVisitor
    {
        void VisitExpression(Expression expression);
    }
}
