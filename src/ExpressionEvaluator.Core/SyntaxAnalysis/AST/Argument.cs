namespace ExpressionEvaluator.SyntaxAnalysis.AST
{
    public class Argument<T>
    {
        public Argument(T expression, SyntaxToken separator)
        {
            this.Expression = expression;
            this.Separator = separator;
        }

        public T Expression { get; private set; }
        public SyntaxToken Separator { get; private set; }
    }
}