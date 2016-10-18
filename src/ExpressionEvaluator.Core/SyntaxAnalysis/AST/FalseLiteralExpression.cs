using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class FalseLiteralExpression : BooleanLiteralExpression
    {
        public FalseLiteralExpression(SyntaxToken token)
            : base(token)
        {

        }
    }
}