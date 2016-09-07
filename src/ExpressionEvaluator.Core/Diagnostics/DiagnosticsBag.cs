using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpressionEvaluator.Diagnostics
{
    public class DiagnosticsBag : IEnumerable<Diagnostic>
    {
        private List<Diagnostic> items = new List<Diagnostic>();

        public void AddInfo(string message, SourceSpan span)
        {
            items.Add(new Diagnostic(DiagnosticType.Info, message, span));
        }

        public void AddWarning(string message, SourceSpan span)
        {
            items.Add(new Diagnostic(DiagnosticType.Warning, message, span));
        }

        public void AddError(string message, SourceSpan span)
        {
            items.Add(new Diagnostic(DiagnosticType.Error, message, span));
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}