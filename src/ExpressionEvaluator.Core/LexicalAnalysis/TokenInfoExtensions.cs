using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.LexicalAnalysis
{
    public static class TokenInfoExtensions
    {
        public static SourceLocation GetStartLocation(this TokenInfo tokenInfo)
        {
            return new SourceLocation(tokenInfo.Line, tokenInfo.Column);
        }

        public static SourceLocation GetEndLocation(this TokenInfo tokenInfo)
        {
            return new SourceLocation(tokenInfo.Line, tokenInfo.Column + tokenInfo.Width);
        }

        public static SourceSpan GetSpan(this TokenInfo tokenInfo)
        {
            return new SourceSpan(tokenInfo.GetStartLocation(), tokenInfo.GetEndLocation());
        }
    }
}
