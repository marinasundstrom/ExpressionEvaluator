namespace ExpressionEvaluator
{
	public enum TokenKind
	{
		Invalid,
		EndOfFile,
		Newline,
		Whitespace,
		Number,
		Plus,
		Minus,
		Star,
		Slash,
		OpenParen,
		CloseParen
	}
}