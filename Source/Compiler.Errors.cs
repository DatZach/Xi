using System;

namespace Xi
{
	internal partial class Compiler
	{
		public void Error(string message, params object[] args)
		{
			string parsedError = String.Format(message, args);
			string errorMessage = String.Format("Error on line {0}: \n\t", stream.CurrentLine);
			errorMessage += parsedError.Replace("\n", "\n\t");

			throw new Exception(errorMessage);
		}

		public void Expected(string value)
		{
			// TODO Stream may need rewinding?
			//--stream.Position;
			Error("\tExpected \"{0}\" got \"{1}\" instead.", value, stream.Peek().Value);
		}
	}
}
