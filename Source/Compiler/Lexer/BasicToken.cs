namespace Xi.Compiler.Lexer
{
	internal class BasicToken
	{
		public BasicTokenType Type { get; private set; }
		public string Value { get; private set; }
		public string Filename { get; private set; }
		public uint Line { get; private set; }

		public BasicToken(BasicTokenType type, string value, string filename, uint line)
		{
			Type = type;
			Value = value;
			Filename = filename;
			Line = line;
		}

		public BasicToken(BasicTokenType type, string filename, uint line)
		{
			Type = type;
			Value = "";
			Filename = filename;
			Line = line;
		}
	}

	enum BasicTokenType
	{
		EndOfStream,
		Unknown,
		Word,
		Number,
		String,
		Delimiter
	}
}
