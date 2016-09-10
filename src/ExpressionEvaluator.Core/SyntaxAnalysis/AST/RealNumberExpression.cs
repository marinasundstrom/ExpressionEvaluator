using ExpressionEvaluator.LexicalAnalysis;
using System.Globalization;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class RealNumberExpression : NumberExpression
	{
		public RealNumberExpression(TokenInfo number, TokenInfo separator, TokenInfo fraction)
		{
            Number = number;
            Separator = separator;
            Fraction = fraction;
		}

		public TokenInfo Number { get; }

        public TokenInfo Separator { get; }

        public TokenInfo Fraction { get; }

        public double Value
        {
            get
            {
                return double.Parse(Number.Value + Separator.Value + Fraction.Value, NumberStyles.AllowDecimalPoint);
            }
        }
	}
}