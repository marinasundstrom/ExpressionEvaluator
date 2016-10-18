using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class IdentifierExpression : Expression
    {
        public IdentifierExpression(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public string Name
        {
            get
            {
                return Token.Value;
            }
        }
    }
}