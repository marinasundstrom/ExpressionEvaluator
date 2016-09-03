namespace ExpressionEvaluator
{
	public class BinaryExpression : Expression
	{
		Expression left;
		BinaryOperation operation;
		Expression right;

		public BinaryExpression(BinaryOperation operation, Expression left, Expression right)
		{
			this.Operation = operation;
			this.Left = left;
			this.Right = right;
		}

		public Expression Left
		{
			get
			{
				return left;
			}

			set
			{
				left = value;
			}
		}

		public BinaryOperation Operation
		{
			get
			{
				return operation;
			}

			set
			{
				operation = value;
			}
		}

		public Expression Right
		{
			get
			{
				return right;
			}

			set
			{
				right = value;
			}
		}
	}
}