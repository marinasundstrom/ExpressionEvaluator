using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class IntegerNumberExpression : NumberExpression
    {
        public IntegerNumberExpression(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public int Value
        {
            get
            {
                return int.Parse(Token.Value);
            }
        }
    }
}