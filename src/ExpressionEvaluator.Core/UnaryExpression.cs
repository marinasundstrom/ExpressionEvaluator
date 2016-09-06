namespace ExpressionEvaluator
{
    internal class UnaryExpression : Expression
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