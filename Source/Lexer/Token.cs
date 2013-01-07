using System.Collections.Generic;

namespace Xi.Lexer
{
	internal class Token
	{
		public TokenType Type { get; private set; }
		public string Value { get; private set; }
		public string Filename { get; private set; }
		public uint Line { get; private set; }

		public Token(TokenType type, string value, string filename, uint line)
		{
			Type = type;
			Value = value;
			Filename = filename;
			Line = line;
		}

		public Token(BasicToken token)
		{
			// TODO Shit hack, will do for now
			Type = new Dictionary<BasicTokenType, TokenType>
			{
				{ BasicTokenType.Number, TokenType.Integer },
				{ BasicTokenType.String, TokenType.String }
			}[token.Type];

			Value = token.Value;
			Filename = token.Filename;
			Line = token.Line;
		}
	}

	internal enum TokenType
	{
		EndOfStream,
		Unknown,
		Word,
		String,
		Integer,
		Double,

		KeywordClass,
		KeywordFunction,
		KeywordVariant,
		Public,
		Private,
		Protected,
		Static,
		Virtual,
		New,
		This,
		Base,
		If,
		Else,
		While,
		Loop,
		For,
		Foreach,
		Do,
		Until,
		Break,
		Continue,
		Negate,
		BinaryNot,
		Modulo,
		BinaryXor,
		BinaryAnd,
		Multiply,
		OpenParentheses,
		CloseParentheses,
		Subtract,
		Add,
		Equal,
		OpenBracket,
		CloseBracket,
		OpenCurlyBracket,
		CloseCurlyBracket,
		BinaryOr,
		Colon,
		Semicolon,
		Ternary,
		Divide,
		Comma,
		Period,
		CompareLessThan,
		CompareGreaterThan,
		CompareEqual,
		CompareNotEqual,
		CompareLessThanOrEqual,
		CompareGreaterThanOrEqual
	}
}
