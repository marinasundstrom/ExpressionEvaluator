using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
namespace ExpressionEvaluator
{
	public static class Evaluator
	{
		public static double EvaluateExpression(Expression expression) 
		{
            var identifier = expression as IdentifierExpression;
            if (identifier != null)
            {
                return 0;
            }
            else
            {
                var number = expression as NumberExpression;
                if (number != null)
                {
                    var integer = expression as IntegerNumberExpression;
                    if (integer != null)
                    {
                        return integer.Value;
                    }
                    else
                    {
                        var real = expression as RealNumberExpression;
                        if (real != null)
                        {
                            return real.Value;
                        }
                    }
                }
                else
                {
                    var parenthesis = expression as ParenthesisExpression;
                    if (parenthesis != null)
                    {
                        return Evaluator.EvaluateExpression(parenthesis.Expression);
                    }
                    else
                    {
                        var binaryExpression = expression as BinaryExpression;
                        if (binaryExpression != null)
                        {
                            var left = Evaluator.EvaluateExpression(binaryExpression.Left);
                            var right = Evaluator.EvaluateExpression(binaryExpression.Right);

                            var operation = ResolveOperation(binaryExpression);

                            switch (operation)
                            {
                                case BinaryOperation.Add:
                                    return left + right;

                                case BinaryOperation.Subtract:
                                    return left - right;

                                case BinaryOperation.Multiply:
                                    return left * right;

                                case BinaryOperation.Divide:
                                    return left / right;

                                case BinaryOperation.Modulo:
                                    return left % right;
                            }
                        }
                    }
                }
            }

			throw new Exception();
		}

        private static BinaryOperation ResolveOperation(BinaryExpression binaryExpression)
        {
            switch(binaryExpression.Operator.Kind)
            {
                case TokenKind.Plus:
                    return BinaryOperation.Add;

                case TokenKind.Minus:
                    return BinaryOperation.Subtract;

                case TokenKind.Star:
                    return BinaryOperation.Multiply;

                case TokenKind.Slash:
                    return BinaryOperation.Divide;

                case TokenKind.Percent:
                    return BinaryOperation.Modulo;
            }

            throw new InvalidOperationException("The operation is not supported.");
        }
    }
}
