using System;

namespace Xi
{
	internal partial class Compiler
	{
		public void Error(string message)
		{
			string errorMessage = String.Format("Error on line {0}: \n\t", stream.CurrentLine);
			errorMessage += message.Replace("\n", "\n\t");

			throw new Exception(errorMessage);
		}

		public void Expected(string value)
		{
			Error(String.Format("\tExpected \"{0}\" got \"{1}\" instead.", value, stream.Peek().Value));
		}
	}
}
