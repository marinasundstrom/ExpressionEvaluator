using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class TrueLiteralExpression : BooleanLiteralExpression
    {
        public TrueLiteralExpression(SyntaxToken token)
            : base(token)
        {
            
        }
    }
}