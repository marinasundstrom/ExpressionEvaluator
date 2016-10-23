using System;
using ExpressionEvaluator.SyntaxAnalysis.AST;

namespace ExpressionEvaluator.SemanticAnalysis
{
    internal class ExpressionInfo : IExpressionInfo
    {
        public ExpressionInfo(Expression expression, ITypeSymbol type)
        {
            Expression = expression;
            Type = type;
        }

        public Expression Expression
        {
            get;
        }

        public ITypeSymbol Type
        {
            get;
        }
    }
}