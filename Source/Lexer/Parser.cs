using System.Collections.Generic;

namespace Xi.Lexer
{
	internal static class Parser
	{
		public static bool IsAddOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "+", "-" }.Contains(token.Value);
		}

		public static bool IsMulOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "*", "/", "%" }.Contains(token.Value);
		}

		public static bool IsAssignOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "=", "+=", "-=", "*=", "/=", "%=", "|=", "^=", "&=", "<<=", ">>=" }.Contains(token.Value);
		}

		public static bool IsBitwiseShiftOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "<<", ">>" }.Contains(token.Value);
		}

		public static bool IsRelationGlOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "<", ">", "<=", ">=" }.Contains(token.Value);
		}

		public static bool IsRelationOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "==", "!=" }.Contains(token.Value);
		}

		public static bool IsLogicalAndOrOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "&&", "||" }.Contains(token.Value);
		}

		public static bool IsBitwiseXorOrOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "^", "|" }.Contains(token.Value);
		}

		public static bool IsIncrementOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Delimiter)
				return false;

			return new List<string> { "++", "--" }.Contains(token.Value);
		}

		public static bool IsTypeCastOperation(Token token)
		{
			if (token == null || token.Type != TokenType.Word)
				return false;

			// TODO Support object casts
			// TODO Should support array casts?
			return new List<string> { "int", "double", "string" }.Contains(token.Value);
		}
	}
}
