using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class UnaryExpression : Expression
    {
        public UnaryExpression(SyntaxToken op, Expression expression)
        {
            this.Operator = op;
            this.Expression = expression;
        }

        public SyntaxToken Operator { get; }

        public Expression Expression { get; }
    }
}