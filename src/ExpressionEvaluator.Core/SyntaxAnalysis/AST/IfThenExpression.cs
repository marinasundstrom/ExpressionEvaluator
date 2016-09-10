using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class IfThenExpression : Expression
    {
        public IfThenExpression(TokenInfo ifKeyword, Expression condition, TokenInfo thenKeyword, Expression body, TokenInfo endKeyword)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenKeyword = thenKeyword;
            Body = body;
            EndKeyword = endKeyword;
        }

        public IfThenExpression(TokenInfo ifKeyword, Expression condition, TokenInfo thenKeyword, Expression body, TokenInfo elseKeyword, Expression elseBody, TokenInfo endKeyword)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenKeyword = thenKeyword;
            Body = body;
            ElseKeyword = elseKeyword;
            ElseBody = elseBody;
            EndKeyword = endKeyword;
        }

        public TokenInfo IfKeyword { get; }

        public Expression Condition { get; }

        public TokenInfo ThenKeyword { get; }

        public Expression Body { get; }

        public TokenInfo ElseKeyword { get; }

        public Expression ElseBody { get; }

        public TokenInfo EndKeyword { get; }
    }
}