using ExpressionEvaluator.LexicalAnalysis;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class Parameter
    {
        public Parameter(TokenInfo name)
        {
            Name = name;
        }

        public TokenInfo Name { get; }
    }
}