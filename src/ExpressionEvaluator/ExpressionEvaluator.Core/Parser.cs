﻿using System;
namespace ExpressionEvaluator
{
	/// <summary>
	/// Parser.
	/// </summary>
	public class Parser
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExpressionEvaluator.Parser"/> class.
		/// </summary>
		/// <param name="lexer">Lexer.</param>
		public Parser(Lexer lexer)
		{
			Lexer = lexer;
		}

		/// <summary>
		/// Gets the lexer.
		/// </summary>
		/// <value>The lexer.</value>
		public Lexer Lexer { get; }

		/// <summary>
		/// Parses an expression.
		/// </summary>
		/// <returns>An expression.</returns>
		public Expression ParseExpression()
		{
			return ParseExpressionCore(0);
		}

		/// <summary>
		/// Parses an expression. (Internal)
		/// </summary>
		/// <returns>An expression.</returns>
		/// <param name="prec">The current level of precendence.</param>
		private Expression ParseExpressionCore(int prec)
		{
			BinaryOperation operation;
			int precendence;

			var expr = ParsePrimaryExpression();

			while (true) 
			{
				var operatorCandidate = Lexer.PeekToken();

				if (TryResolveOperation(operatorCandidate, out operation, out precendence))
				{
					Lexer.ReadToken();

					if (precendence >= prec)
					{
						var right = ParseExpressionCore(prec + 1);

						expr = new BinaryExpression(operation, expr, right);
					}
					else
					{
						return expr;
					}
				}
				else 
				{
					return expr;
				}
			}
		}

		/// <summary>
		/// Tries to resolve an operation from a specified candidate token.
		/// </summary>
		/// <returns><c>true</c>, if the token is an operation, <c>false</c> otherwise.</returns>
		/// <param name="candidateToken">The candidate token for operation.</param>
		/// <param name="operation">The operation corresponding to the candidate token.</param>
		/// <param name="precendence">The operator precendence for the resolved operation.</param>
		private bool TryResolveOperation(TokenInfo candidateToken, out BinaryOperation operation, out int precendence)
		{
			switch (candidateToken.Kind) 
			{
				case TokenKind.Star:
					operation = BinaryOperation.Multiply;
					precendence = 2;
					break;

				case TokenKind.Slash:
					operation = BinaryOperation.Divide;
					precendence = 2;
					break;

				case TokenKind.Plus:
					operation = BinaryOperation.Add;
					precendence = 1;
					break;

				case TokenKind.Minus:
					operation = BinaryOperation.Subtract;
					precendence = 1;
					break;
					
				default:
					operation = BinaryOperation.None;
					precendence = -1;
					return false;
			}

			return true;
		}

		/// <summary>
		/// Parses a primary expression.
		/// </summary>
		/// <returns>An expression.</returns>
		private Expression ParsePrimaryExpression()
		{
			Expression expr = null;

			var token = Lexer.ReadToken();

			switch (token.Kind) {
				case TokenKind.OpenParen:
					expr = ParseExpression();
					token = Lexer.ReadToken();
					if(token.Kind != TokenKind.CloseParen)
						throw new Exception();
					break;

				case TokenKind.Number:
					expr = new NumberExpression(int.Parse(token.Value));
					break;

				default:
					throw new Exception();
			}

			return expr;
		}
	}
}
