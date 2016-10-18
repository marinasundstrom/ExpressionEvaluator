using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(SyntaxToken op, Expression left, Expression right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public Expression Left { get; }

        public SyntaxToken Operator { get; }

        public Expression Right { get; }
    }
}