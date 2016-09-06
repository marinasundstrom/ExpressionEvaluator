namespace ExpressionEvaluator
{
    internal class IdentifierExpression : Expression
    {
        public IdentifierExpression(TokenInfo token)
        {
            Token = token;
        }

        public TokenInfo Token { get; }

        public string Name
        {
            get
            {
                return Token.Value;
            }
        }
    }
}