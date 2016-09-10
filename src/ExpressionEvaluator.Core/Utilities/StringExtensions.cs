using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEvaluator.Utilities
{
    public static class StringExtensions
    {
        public static string Capitalize(this string text)
        {
            text = text.ReplaceAt(0, char.ToUpper(text[0]));          
            return text;
        }

        public static string ReplaceAt(this string value, int index, char newchar)
        {
            if (value.Length <= index)
                return value;
            else
                return string.Concat(value.ToCharArray().Select((c, i) => i == index ? newchar : c));
        }
    }
}
