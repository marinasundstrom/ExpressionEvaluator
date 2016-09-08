using ExpressionEvaluator.LexicalAnalysis;
using System.Globalization;

namespace ExpressionEvaluator.SyntaxAnalysis.AST
{

    public class RealNumberExpression : NumberExpression
	{
		public RealNumberExpression(TokenInfo number, TokenInfo separator, TokenInfo @decimal)
		{
            Number = number;
            Separator = separator;
            Decimal = @decimal;
		}

		public TokenInfo Number { get; }

        public TokenInfo Separator { get; }

        public TokenInfo Decimal { get; }

        public double Value
        {
            get
            {
                return double.Parse(Number.Value + Separator.Value + Decimal.Value, NumberStyles.AllowDecimalPoint);
            }
        }
	}
}