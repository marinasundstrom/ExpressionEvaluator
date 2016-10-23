using ExpressionEvaluator.LexicalAnalysis;
using System.Collections.Generic;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class LetExpression : Expression
    {
        public LetExpression(SyntaxToken letKeyword, SyntaxToken name, SyntaxToken assign, Expression assignedExpression)
        {
            LetKeyword = letKeyword;
            Name = name;
            Assign = assign;
            AssignedExpression = assignedExpression;

            Parameters = new List<Parameter>();
        }

        public Expression AssignedExpression { get; }

        public SyntaxToken Assign { get; }

        public SyntaxToken LetKeyword { get; }

        public SyntaxToken Name { get; }

        public List<Parameter> Parameters { get; }
    }
}