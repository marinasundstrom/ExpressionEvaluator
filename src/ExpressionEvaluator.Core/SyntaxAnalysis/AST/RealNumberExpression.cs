using ExpressionEvaluator.LexicalAnalysis;
using System.Globalization;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class RealNumberExpression : NumberExpression
	{
		public RealNumberExpression(SyntaxToken number, SyntaxToken separator, SyntaxToken fraction)
		{
            Number = number;
            Separator = separator;
            Fraction = fraction;
		}

		public SyntaxToken Number { get; }

        public SyntaxToken Separator { get; }

        public SyntaxToken Fraction { get; }

        public double Value
        {
            get
            {
                return double.Parse(Number.Value + Separator.Value + Fraction.Value, NumberStyles.AllowDecimalPoint);
            }
        }
	}
}