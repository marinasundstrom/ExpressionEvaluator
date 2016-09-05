using System;
using System.IO;
using System.Text;

namespace ExpressionEvaluator
{
	/// <summary>
	/// Lexer.
	/// </summary>
	public class Lexer
	{
		private LexerState? state;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ExpressionEvaluator.Lexer"/> class.
		/// </summary>
		/// <param name="textReader">Text reader.</param>
		public Lexer(TextReader textReader)
		{
			TextReader = textReader;
		}

		/// <summary>
		/// Gets the text reader.
		/// </summary>
		/// <value>The text reader.</value>
		public TextReader TextReader
		{
			get;
		}

		/// <summary>
		/// Peeks a token.
		/// </summary>
		/// <returns>The token.</returns>
		public TokenInfo PeekToken()
		{
			if (state != null)
			{
				var peekedToken = state.Value.Token;
				return peekedToken;
			}
			else 
			{
				var peekToken = ReadTokenCore();
				state = new LexerState(peekToken);
				return peekToken;
			}
		}

		/// <summary>
		/// Reads a token.
		/// </summary>
		/// <returns>The token.</returns>
		public TokenInfo ReadToken()
		{
			if (state != null)
			{
				var token = state.Value.Token;
				state = null;
				return token;
			}
			return ReadTokenCore();
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:ExpressionEvaluator.Lexer"/> has reached EOF.
		/// </summary>
		/// <value><c>true</c> if is EOF; otherwise, <c>false</c>.</value>
		public bool IsEof
		{
			get
			{
				return PeekToken().Kind == TokenKind.EndOfFile;
			}
		}

		/// <summary>
		/// Internal method for reading a token.
		/// </summary>
		/// <returns>The token core.</returns>
		internal TokenInfo ReadTokenCore()
		{
			while (!IsEofCore)
			{
				var c = PeekChar();

				if (char.IsDigit(c))
				{
					var stringBuilder = new StringBuilder();
					do
					{
						ReadChar();

						stringBuilder.Append(c);

						c = PeekChar();
					} while (char.IsDigit(c));

					return new TokenInfo(TokenKind.Number, stringBuilder.Length, stringBuilder.ToString());
				}
				else 
				{
					ReadChar();

					switch (c) 
					{
						case '+':
							return new TokenInfo(TokenKind.Plus, 1);

						case '-':
							return new TokenInfo(TokenKind.Minus, 1);

						case '*':
							return new TokenInfo(TokenKind.Star, 1);

						case '/':
							return new TokenInfo(TokenKind.Slash, 1);

						case '(':
							return new TokenInfo(TokenKind.OpenParen, 1);

						case ')':
							return new TokenInfo(TokenKind.CloseParen, 1);

						case ' ':
							//return new TokenInfo(TokenKind.Whitespace, 1);
							break;

						default:
						return new TokenInfo(TokenKind.Invalid, 1, c.ToString());
					}
				}
			}

			return new TokenInfo(TokenKind.EndOfFile, 0);
		}

		private bool IsEofCore
		{
			get
			{
				return TextReader.Peek() == -1;
			}
		}

		private char ReadChar()
		{
			return (char)TextReader.Read();
		}

		private char PeekChar()
		{
			return (char)TextReader.Peek();
		}

		/// <summary>
		/// Encapsulates a lookahead state for the Lexer..
		/// </summary>
		struct LexerState
		{
			public LexerState(TokenInfo token)
			{
				Token = token;
			}

			public TokenInfo Token { get;}
		}
	}
}
