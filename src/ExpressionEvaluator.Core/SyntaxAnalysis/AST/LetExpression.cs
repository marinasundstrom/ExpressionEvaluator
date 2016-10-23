using ExpressionEvaluator.LexicalAnalysis;
using System.Collections.Generic;
using System.Linq;

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
        public System.Boolean HasParameters
        {
            get { return !Parameters.Any(); }
        }
    }
}