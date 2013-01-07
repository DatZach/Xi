using System;
using Xi.Lexer;

namespace Xi
{
	internal static class Errors
	{
		public static void Error(string message)
		{
			
		}

		public static void Expected(TokenType type)
		{
			Console.WriteLine("Something bad happened...");
			//Console.WriteLine("Error on line {0}:\n\tExpected \"{1}\" got \"{2}\" instead.", 0, 0, type.Type);
		}
	}
}
