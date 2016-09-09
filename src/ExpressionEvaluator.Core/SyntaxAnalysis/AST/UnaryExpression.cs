using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class UnaryExpression : Expression
    {
        public UnaryExpression(TokenInfo op, Expression expression)
        {
            this.Operator = op;
            this.Expression = expression;
        }

        public TokenInfo Operator { get; }

        public Expression Expression { get; }
    }
}