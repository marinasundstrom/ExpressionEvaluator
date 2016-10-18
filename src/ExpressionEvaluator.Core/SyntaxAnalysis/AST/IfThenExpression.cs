using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class IfThenExpression : Expression
    {
        public IfThenExpression(SyntaxToken ifKeyword, Expression condition, SyntaxToken thenKeyword, Expression body, SyntaxToken endKeyword)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenKeyword = thenKeyword;
            Body = body;
            EndKeyword = endKeyword;
        }

        public IfThenExpression(SyntaxToken ifKeyword, Expression condition, SyntaxToken thenKeyword, Expression body, SyntaxToken elseKeyword, Expression elseBody, SyntaxToken endKeyword)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenKeyword = thenKeyword;
            Body = body;
            ElseKeyword = elseKeyword;
            ElseBody = elseBody;
            EndKeyword = endKeyword;
        }

        public SyntaxToken IfKeyword { get; }

        public Expression Condition { get; }

        public SyntaxToken ThenKeyword { get; }

        public Expression Body { get; }

        public SyntaxToken ElseKeyword { get; }

        public Expression ElseBody { get; }

        public SyntaxToken EndKeyword { get; }
    }
}