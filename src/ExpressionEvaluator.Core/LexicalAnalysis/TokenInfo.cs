namespace ExpressionEvaluator.LexicalAnalysis
{
	public struct TokenInfo
	{
		public TokenInfo(TokenKind kind, int line, int column, int width) 
			: this(kind, line, column, width, string.Empty)
		{ 

		}

		public TokenInfo(TokenKind kind, int line, int column, int width, string value)
		{
			Kind = kind;
            Line = line;
            Column = column;
			Width = width;
			Value = value;
		}

		public TokenKind Kind { get; }

		public string Value { get; }

        public int Line { get; }

        public int Column { get; }

        public int Width { get; }

        public static TokenInfo Empty { get; } = default(TokenInfo);
    }
}