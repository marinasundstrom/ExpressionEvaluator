using ExpressionEvaluator.LexicalAnalysis;
using ExpressionEvaluator.SyntaxAnalysis;
using ExpressionEvaluator.SyntaxAnalysis.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.CodeGen
{
    public class CodeGenerator : ITreeVisitor
    {
        ILGenerator gen;

        public Func<double> Generate(Expression expression)
        {
            var method = new DynamicMethod("Function", typeof(double), new Type[0]);
            gen = method.GetILGenerator();

            ((ITreeVisitor)this).VisitExpression(expression);

            gen.Emit(OpCodes.Ret);

            return (Func<double>)method.CreateDelegate(typeof(Func<double>));
        }

        void ITreeVisitor.VisitExpression(Expression expression)
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
                        ((ITreeVisitor)this).VisitExpression(parenthesis.Expression);
                    }
                    else
                    {
                        var binaryExpression = expression as BinaryExpression;
                        if (binaryExpression != null)
                        {
                            ((ITreeVisitor)this).VisitExpression(binaryExpression.Left);
                            ((ITreeVisitor)this).VisitExpression(binaryExpression.Right);

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
                        else
                        {
                            var unaryExpression = expression as UnaryExpression;
                            if (unaryExpression != null)
                            {
                                ((ITreeVisitor)this).VisitExpression(unaryExpression.Expression);

                                var operation = ResolveOperation(unaryExpression);

                                switch (operation)
                                {
                                    case BinaryOperation.Negative:
                                        gen.Emit(OpCodes.Neg);
                                        break;
                                }
                            }
                            else
                            {
                                var ifExpression = expression as IfThenExpression;
                                if (ifExpression != null)
                                {
                                    var endElseLabel = gen.DefineLabel();

                                    ((ITreeVisitor)this).VisitExpression(ifExpression.Condition);

                                    gen.Emit(OpCodes.Brfalse, endElseLabel);

                                    ((ITreeVisitor)this).VisitExpression(ifExpression.Body);

                                    gen.MarkLabel(endElseLabel);

                                    ((ITreeVisitor)this).VisitExpression(ifExpression.ElseBody);
                                }
                                else
                                {
                                    var letExpression = expression as LetExpression;
                                    if (letExpression != null)
                                    {
                                        if(letExpression.HasParameters)
                                        {

                                        }
                                        else
                                        {
                                            var local = gen.DeclareLocal(typeof(double));

                                            ((ITreeVisitor)this).VisitExpression(ifExpression.ElseBody);
                                            gen.Emit(OpCodes.Stloc, local);
                                        }
                                    }
                                }
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
                case SyntaxKind.Plus:
                    return BinaryOperation.Add;

                case SyntaxKind.Minus:
                    return BinaryOperation.Subtract;

                case SyntaxKind.Star:
                    return BinaryOperation.Multiply;

                case SyntaxKind.Slash:
                    return BinaryOperation.Divide;

                case SyntaxKind.Percent:
                    return BinaryOperation.Modulo;

                case SyntaxKind.Caret:
                    return BinaryOperation.Power;
            }

            throw new InvalidOperationException("The operation is not supported.");
        }

        private static BinaryOperation ResolveOperation(UnaryExpression unaryExpression)
        {
            switch (unaryExpression.Operator.Kind)
            {
                case SyntaxKind.Minus:
                    return BinaryOperation.Negative;

                case SyntaxKind.Plus:
                    return BinaryOperation.Postive;
            }

            throw new InvalidOperationException("The operation is not supported.");
        }
    }
}
