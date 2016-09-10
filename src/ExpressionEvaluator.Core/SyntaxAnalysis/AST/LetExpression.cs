using ExpressionEvaluator.LexicalAnalysis;
using System.Collections.Generic;

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

            Parameters = new List<Parameter>();
        }

        public Expression AssignedExpression { get; }

        public TokenInfo Assign { get; }

        public TokenInfo LetKeyword { get; }

        public TokenInfo Name { get; }

        public List<Parameter> Parameters { get; }
    }
}