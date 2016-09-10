using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public abstract class BooleanLiteralExpression : Expression
    {
        private TokenInfo token;

        public BooleanLiteralExpression(TokenInfo token)
        {
            this.token = token;
        }
    }
}