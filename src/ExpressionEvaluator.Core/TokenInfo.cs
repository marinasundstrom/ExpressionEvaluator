namespace ExpressionEvaluator
{
	public struct TokenInfo
	{
		public TokenInfo(TokenKind kind, int width) 
			: this(kind, width, string.Empty)
		{ 

		}

		public TokenInfo(TokenKind kind, int width, string value)
		{
			Kind = kind;
			Width = width;
			Value = value;
		}

		public TokenKind Kind { get; }

		public string Value { get; }

		public int Width { get; }
	}
}