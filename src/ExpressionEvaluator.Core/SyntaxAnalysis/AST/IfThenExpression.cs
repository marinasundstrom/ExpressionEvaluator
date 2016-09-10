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

        public TokenInfo IfKeyword { get; }

        public Expression Condition { get; }

        public TokenInfo ThenKeyword { get; }

        public Expression Body { get; }

        public TokenInfo EndKeyword { get; }
    }
}