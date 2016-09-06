namespace ExpressionEvaluator
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(TokenInfo op, Expression left, Expression right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public Expression Left { get; }

        public TokenInfo Operator { get; }

        public Expression Right { get; }
    }
}