namespace ExpressionEvaluator
{
	public class NumberExpression : Expression
	{
		public NumberExpression(TokenInfo token)
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