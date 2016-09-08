using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class IntegerNumberExpression : NumberExpression
    {
        public IntegerNumberExpression(TokenInfo token)
        {
            Token = token;
        }

        public TokenInfo Token { get; }

        public int Value
        {
            get
            {
                return int.Parse(Token.Value);
            }
        }
    }
}