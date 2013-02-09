namespace Xi.Util
{
	/*
	 * Crappy implementation of a StringStream
	 * Still less crappy than any default .NET implementations of a string stream
	 */

	class StringStream
	{
		public const char NewLine = '\n';

		public string Value { get; private set; }
		public int Position;

		public bool IsEndOfStream
		{
			get { return Position >= Value.Length; }
		}

		public StringStream(string value)
		{
			Value = value;
			Position = 0;
		}

		public char Peek()
		{
			return Position < Value.Length ? Value[Position] : '\0';
		}

		public char PeekAhead(int offset)
		{
			return Position + offset < Value.Length ? Value[Position + offset] : '\0';
		}

		public char Read()
		{
			return Position < Value.Length ? Value[Position++] : '\0';
		}
	}
}
