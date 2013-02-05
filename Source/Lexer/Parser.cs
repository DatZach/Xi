using System;
using System.Collections.Generic;
using System.IO;
using Xi.Util;

namespace Xi.Lexer
{
	internal static class Parser
	{
		private static readonly Dictionary<string, TokenType> keywords, delimiters;

		static Parser()
		{
			keywords = new Dictionary<string, TokenType>
			{
				// Top level constructs
				{ "class", TokenType.KeywordClass },
				{ "function", TokenType.KeywordFunction },
				{ "var", TokenType.KeywordVariant },

				// Visibility
				{ "public", TokenType.Public },
				{ "private", TokenType.Private },
				{ "protected", TokenType.Protected },

				// Misc
				{ "static", TokenType.Static },
				{ "virtual", TokenType.Virtual },
				{ "new", TokenType.New },
				{ "this", TokenType.This },
				{ "base", TokenType.Base },

				// Conditional Statements
				{ "if", TokenType.If },
				{ "else", TokenType.Else },
				{ "while", TokenType.While },
				{ "loop", TokenType.Loop },
				{ "for", TokenType.For },
				{ "foreach", TokenType.Foreach },
				{ "do", TokenType.Do },
				{ "until", TokenType.Until },
				{ "break", TokenType.Break },
				{ "continue", TokenType.Continue }
			};

			delimiters = new Dictionary<string, TokenType>
			{
				{ "~", TokenType.Negate },
				{ "!", TokenType.BinaryNot },
				{ "%", TokenType.Modulo },
				{ "^", TokenType.BinaryXor },
				{ "&", TokenType.BinaryAnd },
				{ "*", TokenType.Multiply },
				{ "(", TokenType.OpenParentheses },
				{ ")", TokenType.CloseParentheses },
				{ "-", TokenType.Subtract },
				{ "+", TokenType.Add },
				{ "=", TokenType.Equal },
				{ "[", TokenType.OpenBracket },
				{ "]", TokenType.CloseBracket },
				{ "{", TokenType.OpenCurlyBracket },
				{ "}", TokenType.CloseCurlyBracket },
				{ "|", TokenType.BinaryOr },
				{ ":", TokenType.Colon },
				{ ";", TokenType.Semicolon },
				{ "?", TokenType.Ternary },
				{ "/", TokenType.Divide },
				{ ",", TokenType.Comma },
				{ ".", TokenType.Period },
				{ "<", TokenType.CompareLessThan },
				{ ">", TokenType.CompareGreaterThan },
				{ "==", TokenType.CompareEqual },
				{ "!=", TokenType.CompareNotEqual },
				{ "<=", TokenType.CompareLessThanOrEqual },
				{ ">=", TokenType.CompareGreaterThanOrEqual }
			};
		}

		public static List<Token> ParseString(string value)
		{
			List<BasicToken> basicTokens = Tokenizer.ParseStream(new StringStream(value), "");
			return ParseStream(basicTokens);
		}

		public static List<Token> ParseFile(string filename)
		{
			using (StreamReader reader = new StreamReader(filename))
			{
				List<BasicToken> basicTokens = Tokenizer.ParseStream(new StringStream(reader.ReadToEnd()), filename);
				return ParseStream(basicTokens);
			}
		}

		private static List<Token> ParseStream(IEnumerable<BasicToken> stream)
		{
			List<Token> tokens = new List<Token>();

			foreach (BasicToken t in stream)
			{
				switch (t.Type)
				{
					case BasicTokenType.Word:
						tokens.Add(keywords.ContainsKey(t.Value)
							           ? new Token(keywords[t.Value], "", t.Filename, t.Line)
							           : new Token(TokenType.Word, t.Value, t.Filename, t.Line));
						break;

					case BasicTokenType.Delimiter:
						tokens.Add(new Token(delimiters[t.Value], "", t.Filename, t.Line));
						break;

					case BasicTokenType.String:
					case BasicTokenType.Number:
						tokens.Add(new Token(t));
						break;

					case BasicTokenType.EndOfStream:
						tokens.Add(new Token(TokenType.EndOfStream, "", t.Filename, t.Line));
						break;

					case BasicTokenType.Unknown:
						throw new Exception("Unknown token \"" + t.Value + "\"!");
				}
			}

			return tokens;
		}

		public static bool IsAddOperation(Token token)
		{
			if (token == null)
				return false;

			return new List<TokenType> { TokenType.Add, TokenType.Subtract }.Contains(token.Type);
		}

		public static bool IsMulOperation(Token token)
		{
			if (token == null)
				return false;

			return new List<TokenType> { TokenType.Multiply, TokenType.Divide, TokenType.Modulo }.Contains(token.Type);
		}
	}
}
