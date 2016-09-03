namespace ExpressionEvaluator
{
	public class NumberExpression : Expression
	{
		readonly int v;

		public NumberExpression(int v)
		{
			this.v = v;
		}

		public int Value
		{
			get
			{
				return v;
			}
		}
	}
}