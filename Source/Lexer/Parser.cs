using System.Collections.Generic;

namespace Xi.Lexer
{
	internal static class Parser
	{
		// TODO Should probably move this elsewhere

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
	}
}
