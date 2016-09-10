using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class FalseLiteralExpression : BooleanLiteralExpression
    {
        public FalseLiteralExpression(TokenInfo token)
            : base(token)
        {

        }
    }
}