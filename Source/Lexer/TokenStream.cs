using System.Collections.Generic;
using System.Globalization;
using Xi.Vm;

namespace Xi.Lexer
{
	internal class TokenStream
	{
		private readonly List<Token> tokens;
		public int Position;

		public bool IsEndOfStream
		{
			get { return Position >= tokens.Count; }
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
			if (Read().Type == type)
				return;

			Errors.Expected(type);
		}

		public bool Accept(TokenType type)
		{
			Token token = Peek();
			if (token.Type == type)
			{
				Read();
				return true;
			}

			return false;
		}

		public string GetWord()
		{
			Token token = Read();
			if (token.Type != TokenType.Word)
			{
				Errors.Expected(TokenType.Word);
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

				case TokenType.Integer:
				{
					long value;
					long.TryParse(token.Value, NumberStyles.Number, null, out value);

					return new Variant(value);
				}

				case TokenType.Double:
				{
					double value;
					double.TryParse(token.Value, NumberStyles.Number, null, out value);

					return new Variant(value);
				}
			}

			// Should this error?
			Errors.Expected(TokenType.Unknown);

			return new Variant();
		}
	}
}
