namespace ExpressionEvaluator
{
    public struct SourceLocation
    {
        public SourceLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Column { get; private set; }

        public int Line { get; private set; }

        public override string ToString()
        {
            return $"{Column}:{Line}";
        }
    }
}