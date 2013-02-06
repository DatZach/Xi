using System;

namespace Xi
{
	internal partial class Compiler
	{
		public void Error(string message)
		{

		}

		public void Expected(string value)
		{
			Console.WriteLine("Error on line {0}:\n\tExpected \"{1}\" got \"{2}\" instead.", stream.CurrentLine, value, stream.Peek().Value);
		}
	}
}
