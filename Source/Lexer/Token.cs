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

		public Token(TokenType type, string filename, uint line)
		{
			Type = type;
			Value = "";
			Filename = filename;
			Line = line;
		}
	}

	internal enum TokenType
	{
		EndOfStream,
		Unknown,
		Word,
		Number,
		String,
		Delimiter
	}
}
