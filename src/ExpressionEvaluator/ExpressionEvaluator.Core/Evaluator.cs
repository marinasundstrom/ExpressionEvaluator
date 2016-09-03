using System;
namespace ExpressionEvaluator
{
	public class Evaluator
	{
		public Evaluator()
		{
		}

		public static int EvaluateExpression(Expression expression) 
		{
			var number = expression as NumberExpression;
			if (number != null)
			{
				return number.Value;
			}
			else 
			{ 
				var binaryExpression = expression as BinaryExpression;
				if (binaryExpression != null)
				{
					var left = Evaluator.EvaluateExpression(binaryExpression.Left);
					var right = Evaluator.EvaluateExpression(binaryExpression.Right);

					switch (binaryExpression.Operation)
					{
						case BinaryOperation.Add:
							return left + right;

						case BinaryOperation.Subtract:
							return left - right;

						case BinaryOperation.Multiply:
							return left * right;

						case BinaryOperation.Divide:
							return left / right;

					}
				}
			}

			throw new Exception();
		}
	}
}
