using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public abstract class BooleanLiteralExpression : Expression
    {
        private SyntaxToken token;

        public BooleanLiteralExpression(SyntaxToken token)
        {
            this.token = token;
        }
    }
}