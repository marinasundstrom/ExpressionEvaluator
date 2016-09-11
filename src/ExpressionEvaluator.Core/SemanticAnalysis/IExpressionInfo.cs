using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;

namespace ExpressionEvaluator.SemanticAnalysis
{

    public interface IExpressionInfo
    {
        Expression Expression { get; }

        ITypeSymbol Type { get; }
    }
}