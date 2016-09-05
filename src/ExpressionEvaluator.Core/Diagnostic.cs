namespace ExpressionEvaluator
{
    public class Diagnostic
    {
        public Diagnostic(DiagnosticType type, string message, SourceSpan span)
        {
            Type = type;
            Message = message;
            Span = span;
        }

        public DiagnosticType Type { get; }

        public string Message { get; }

        public SourceSpan Span { get; }
    }
}