using System;
using Xi.Lexer;

namespace Xi
{
	internal partial class Compiler
	{
		public void Error(string message)
		{

		}

		public void Expected(TokenType type)
		{
			Console.WriteLine("Error on line {0}:\n\tExpected \"{1}\" got \"{2}\" instead.", stream.CurrentLine, stream.Peek(), type);
		}
	}
}
