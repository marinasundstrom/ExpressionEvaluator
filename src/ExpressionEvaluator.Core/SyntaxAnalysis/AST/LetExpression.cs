using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    internal class LetExpression : Expression
    {
        public LetExpression(TokenInfo letKeyword, TokenInfo name, TokenInfo assign, Expression assignedExpression)
        {
            LetKeyword = letKeyword;
            Name = name;
            Assign = assign;
            AssignedExpression = assignedExpression;
        }

        public Expression AssignedExpression { get; }

        public TokenInfo Assign { get; }

        public TokenInfo LetKeyword { get; }

        public TokenInfo Name { get; }
    }
}