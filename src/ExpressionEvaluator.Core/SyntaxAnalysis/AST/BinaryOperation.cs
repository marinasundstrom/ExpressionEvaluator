namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
	public enum BinaryOperation
	{
		None,
		Add,
		Subtract,
		Multiply,
		Divide,
        Is,
        IsNot,
        Equal,
        NotEquals,
        And,
        Or,
        Less,
        Greater,
        GreaterOrEquals,
        LessOrEquals,
        Modulo
    }
}