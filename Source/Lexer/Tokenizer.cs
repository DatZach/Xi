using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xi.Util;

namespace Xi.Lexer
{
	internal static class Tokenizer
	{
		private const string Delimiters = "~!%^&*()-+=[]{}|:;?/<,>.";

		public static List<BasicToken> ParseStream(StringStream stream, string filename)
		{
			List<BasicToken> tokens = new List<BasicToken>();
			uint line = 1;

			while (!stream.IsEndOfStream)
			{
				// Skip whitespace
				while (!stream.IsEndOfStream && char.IsWhiteSpace(stream.Peek()))
				{
					// Read new lines
					if (stream.Read() == '\n')
						++line;
				}

				// Skip single line comments
				if (stream.Peek() == '/' && stream.PeekAhead(1) == '/')
				{
					while (!stream.IsEndOfStream && stream.Peek() != '\n')
						stream.Read();

					continue;
				}

				// Skip multi line comments
				if (stream.Peek() == '/' && stream.PeekAhead(1) == '*')
				{
					while (!stream.IsEndOfStream && !(stream.Peek() == '*' && stream.PeekAhead(1) == '/'))
					{
						if (stream.Read() == '\n')
							++line;
					}
				}

				// Parse delimiters
				if (Delimiters.Contains(stream.Peek()))
				{
					tokens.Add(new BasicToken(BasicTokenType.Delimiter, "" + stream.Read(), filename, line));
					continue;
				}

				// TODO Proper multiline strings
				// Parse strings
				if (stream.Peek() == '\"')
				{
					uint startLine = line;
					string value = "";
					char ch;

					++stream.Position;
					while ((ch = stream.Read()) != '\"')
					{
						if (stream.IsEndOfStream)
							throw new Exception("Unterminated string on line " + startLine);

						if (ch == '\n')
							++line;

						value += ch;
					}

					tokens.Add(new BasicToken(BasicTokenType.String, value, filename, startLine));
					//++stream.Position;
					continue;
				}

				// Parse words
				if (char.IsLetter(stream.Peek()) || stream.Peek() == '_')
				{
					string word = "";

					while (!stream.IsEndOfStream && (char.IsLetterOrDigit(stream.Peek()) || stream.Peek() == '_'))
						word += stream.Read();

					tokens.Add(new BasicToken(BasicTokenType.Word, word, filename, line));
					continue;
				}

				// Parse numbers
				if (char.IsDigit(stream.Peek()))
				{
					string number = "";

					if (stream.Peek() == '0' && stream.PeekAhead(1) == 'x')
					{
						stream.Position += 2;

						while (!stream.IsEndOfStream && char.IsLetterOrDigit(stream.Peek()))
							number += stream.Read();

						ulong result;
						try
						{
							result = ulong.Parse(number, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
						}
						catch (OverflowException)
						{
							throw new Exception("Number is larger than 64bits on line " + line);
						}
						catch (Exception)
						{
							throw new Exception("Invalid number on line " + line);
						}

						number = result.ToString("G");
					}
					else
					{
						while (!stream.IsEndOfStream && (char.IsDigit(stream.Peek()) || stream.Peek() == '.'))
							number += stream.Read();
					}

					tokens.Add(new BasicToken(BasicTokenType.Number, number, filename, line));
					continue;
				}

				// Don't run off the rails
				if (stream.IsEndOfStream)
					break;

				throw new Exception("Unexpected token \"" + stream.Read() + "\" on line " + line);
			}

			tokens.Add(new BasicToken(BasicTokenType.EndOfStream, filename, line));

			return tokens;
		}
	}
}
