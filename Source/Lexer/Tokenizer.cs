using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xi.Util;

namespace Xi.Lexer
{
	internal static class Tokenizer
	{
		// NOTE Make sure that the largest delimiters come first in the list
		private static readonly List<string> delimiters = new List<string>
		{
			"<<=", ">>=",
			"+=", "-=","*=", "/=", "%=", "|=", "^=", "&=", "==", "!=", "<=", ">=", "<<", ">>", "&&", "||", "++", "--",
			"~", "!", "%", "^", "&", "*", "(", ")", "-", "+", "=", "[", "]", "{", "}", "|", ":", ";", "?", "/", "<", ",", ">", "."
		};

		private static string filename;
		private static uint line;

		public static List<Token> ParseString(string value)
		{
			filename = "";

			try
			{
				return ParseStream(new StringStream(value));
			}
			catch (Exception e)
			{
				Console.WriteLine("Lexer Error: {0}", e.Message);
				return null;
			}
		}

		public static List<Token> ParseFile(string fname)
		{
			filename = fname;

			try
			{
				using (StreamReader reader = new StreamReader(fname))
				{
					return ParseStream(new StringStream(reader.ReadToEnd()));
				}
			}
			catch (IOException)
			{
				Console.WriteLine("Error: Cannot open file \"{0}\" for reading.", filename);
				return null;
			}
			catch (Exception e)
			{
				Console.WriteLine("Lexer Error: {0}", e.Message);
				return null;
			}
		}

		private static List<Token> ParseStream(StringStream stream)
		{
			List<Token> tokens = new List<Token>();
			line = 1;

			while (!stream.IsEndOfStream)
			{
				SkipWhitespace(stream);

				if (SkipComment(stream))
					continue;

				if (ParseString(stream, tokens))
					continue;

				if (ParseWord(stream, tokens))
					continue;

				if (ParseNumber(stream, tokens))
					continue;

				if (ParseDelimiter(stream, tokens))
					continue;

				if (stream.IsEndOfStream)
					break;

				throw new Exception("Unexpected token \"" + stream.Read() + "\" on line " + line);
			}

			tokens.Add(new Token(TokenType.EndOfStream, filename, line));

			return tokens;
		}

		private static void SkipWhitespace(StringStream stream)
		{
			while (!stream.IsEndOfStream && char.IsWhiteSpace(stream.Peek()))
			{
				// Read new lines
				if (stream.Read() == StringStream.NewLine)
					++line;
			}
		}

		private static bool SkipComment(StringStream stream)
		{
			// Skip single line comments
			if (stream.Peek() == '/' && stream.PeekAhead(1) == '/')
			{
				while (!stream.IsEndOfStream && stream.Peek() != StringStream.NewLine)
					stream.Read();

				return true;
			}

			// Skip multi line comments
			if (stream.Peek() == '/' && stream.PeekAhead(1) == '*')
			{
				while (!stream.IsEndOfStream && !(stream.Peek() == '*' && stream.PeekAhead(1) == '/'))
				{
					if (stream.Read() == StringStream.NewLine)
						++line;
				}

				stream.Position += 2;
				return true;
			}

			return false;
		}

		private static bool ParseString(StringStream stream, List<Token> tokens)
		{
			if (stream.Peek() != '\"')
				return false;

			uint startLine = line;
			string value = "";

			while (stream.Peek() == '\"')
			{
				char ch;

				++stream.Position;
				while ((ch = stream.Read()) != '\"')
				{
					if (stream.IsEndOfStream)
						throw new Exception("Unterminated string on line " + startLine);

					if (ch == StringStream.NewLine)
						++line;

					value += ch;
				}

				SkipWhitespace(stream);
			}

			tokens.Add(new Token(TokenType.String, value, filename, startLine));

			return true;
		}

		private static bool ParseWord(StringStream stream, List<Token> tokens)
		{
			if (char.IsLetter(stream.Peek()) || stream.Peek() == '_')
			{
				string word = "";

				while (!stream.IsEndOfStream && (char.IsLetterOrDigit(stream.Peek()) || stream.Peek() == '_'))
					word += stream.Read();

				tokens.Add(new Token(TokenType.Word, word, filename, line));
				return true;
			}

			return false;
		}

		private static bool ParseNumber(StringStream stream, List<Token> tokens)
		{
			if (char.IsDigit(stream.Peek()) || (stream.Peek() == '-' && char.IsDigit(stream.PeekAhead(1))))
			{
				string number = "";
				if (stream.Peek() == '-')
					number += stream.Read();

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

				tokens.Add(new Token(TokenType.Number, number, filename, line));
				return true;
			}

			return false;
		}

		private static bool ParseDelimiter(StringStream stream, List<Token> tokens)
		{
			bool foundDelimiter = false;
			string peekedDelimiter = "";
			foreach (string del in delimiters)
			{
				peekedDelimiter = "";
				for (int i = 0; i < del.Length; ++i)
					peekedDelimiter += stream.PeekAhead(i);

				if (peekedDelimiter == del)
				{
					stream.Position += del.Length;
					foundDelimiter = true;
					break;
				}
			}

			if (foundDelimiter)
			{
				tokens.Add(new Token(TokenType.Delimiter, peekedDelimiter, filename, line));
				return true;
			}

			return false;
		}
	}
}
