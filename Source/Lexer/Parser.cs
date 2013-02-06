using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xi.Lexer
{
	class Parser
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
	}
}
