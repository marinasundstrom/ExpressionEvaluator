using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class Parameter
    {
        public Parameter(SyntaxToken name)
        {
            Name = name;
        }

        public SyntaxToken Name { get; }
    }
}