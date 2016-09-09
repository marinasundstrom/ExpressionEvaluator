using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.CodeGen
{
    public static class CodeGenerator
    {
        public static Func<double> Generate(Expression expression)
        {
            var method = new DynamicMethod("Function", typeof(double), new Type[0]);
            var gen = method.GetILGenerator();
            GenerateExpression(gen, expression);

            gen.Emit(OpCodes.Ret);

            return (Func<double>)method.CreateDelegate(typeof(Func<double>));
        }

        public static void GenerateExpression(ILGenerator gen, Expression expression)
        {
            var identifier = expression as IdentifierExpression;
            if (identifier != null)
            {
                gen.Emit(OpCodes.Ldc_I4_0);
            }
            else
            {
                var number = expression as NumberExpression;
                if (number != null)
                {
                    var integer = expression as IntegerNumberExpression;
                    if (integer != null)
                    {
                        gen.Emit(OpCodes.Ldc_I4, integer.Value);
                        gen.Emit(OpCodes.Conv_R8);
                    }
                    else
                    {
                        var real = expression as RealNumberExpression;
                        if (real != null)
                        {
                            gen.Emit(OpCodes.Ldc_R8, real.Value);
                        }
                    }
                }
                else
                {
                    var parenthesis = expression as ParenthesisExpression;
                    if (parenthesis != null)
                    {
                        Evaluator.EvaluateExpression(parenthesis.Expression);
                    }
                    else
                    {
                        var binaryExpression = expression as BinaryExpression;
                        if (binaryExpression != null)
                        {
                            GenerateExpression(gen, binaryExpression.Left);
                            GenerateExpression(gen, binaryExpression.Right);

                            var operation = ResolveOperation(binaryExpression);

                            switch (operation)
                            {
                                case BinaryOperation.Add:
                                    gen.Emit(OpCodes.Add);
                                    break;

                                case BinaryOperation.Subtract:
                                    gen.Emit(OpCodes.Sub);
                                    break;

                                case BinaryOperation.Multiply:
                                    gen.Emit(OpCodes.Mul);
                                    break;

                                case BinaryOperation.Divide:
                                    gen.Emit(OpCodes.Div);
                                    break;

                                case BinaryOperation.Modulo:
                                    gen.Emit(OpCodes.Rem);
                                    break;

                                case BinaryOperation.Power:
                                    var methodInfo = typeof(Math).GetMethod("Pow");
                                    gen.EmitCall(OpCodes.Call, methodInfo, new Type[] { typeof(double), typeof(double) });
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static BinaryOperation ResolveOperation(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.Operator.Kind)
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

                case TokenKind.Caret:
                    return BinaryOperation.Power;
            }

            throw new InvalidOperationException("The operation is not supported.");
        }
    }
}
