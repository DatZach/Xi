using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xi.Vm;

namespace Xi.Lexer
{
	internal class TokenStream
	{
		private readonly List<Token> tokens;
		public int Position;

		public bool IsEndOfStream
		{
			get { return Position >= tokens.Count || tokens[Position].Type == TokenType.EndOfStream; }
		}

		public uint CurrentLine
		{
			get
			{
				return Peek() == null ? tokens.Last().Line : Peek().Line;
			}
		}

		public TokenStream(List<Token> tokens)
		{
			this.tokens = tokens;
			Position = 0;
		}

		public Token Peek()
		{
			return IsEndOfStream ? null : tokens[Position];
		}

		public Token PeekAhead(int offset)
		{
			return Position + offset < tokens.Count ? tokens[Position + offset] : null;
		}

		public Token Read()
		{
			return IsEndOfStream ? null : tokens[Position++];
		}

		public void Expect(TokenType type)
		{
			if (!IsEndOfStream && Read().Type == type)
				return;

			// TODO Fix errors somehow...
			Expected(type.ToString());
		}

		public void Expect(TokenType type, string value)
		{
			if (!IsEndOfStream)
			{
				Token token = Read();
				if (token.Type == type && token.Value == value)
					return;
			}

			// TODO Fix errors somehow...
			Expected(value);
		}

		public bool Accept(TokenType type)
		{
			if (IsEndOfStream)
				return false;

			Token token = Peek();
			if (token.Type == type)
			{
				Read();
				return true;
			}

			return false;
		}

		public bool Accept(TokenType type, string value)
		{
			if (IsEndOfStream)
				return false;

			Token token = Peek();
			if (token.Type == type && token.Value == value)
			{
				Read();
				return true;
			}

			return false;
		}

		public bool Pass(TokenType type)
		{
			return Peek().Type == type;
		}

		public string GetWord()
		{
			Token token = Read();
			if (token.Type != TokenType.Word)
			{
				// TODO Fix this
				Expected("word");
				return "";
			}

			return token.Value;
		}

		public Variant GetVariant()
		{
			Token token = Read();
			switch (token.Type)
			{
				case TokenType.String:
					return new Variant(token.Value);

				case TokenType.Number:
				{
					if (token.Value.Contains("."))
					{
						double value;
						if (!double.TryParse(token.Value, NumberStyles.Number, null, out value))
							return new Variant(0.0);

						return new Variant(value);
					}
					else
					{
						long value;
						if (!long.TryParse(token.Value, NumberStyles.Number, null, out value))
							return new Variant(0);

						return new Variant(value);
					}
				}
			}

			// Should this error?
			// TODO Fix this
			Expected("variant");

			return new Variant();
		}

		// TODO fix this redundency
		public void Expected(string value)
		{
			Token peekedToken = Peek();
			string peekedValue = peekedToken == null ? "" : peekedToken.Value;

			throw new Exception(String.Format("Error on line {0}:\n\tExpected \"{1}\" got \"{2}\" instead.", CurrentLine, value, peekedValue));
		}
	}
}
