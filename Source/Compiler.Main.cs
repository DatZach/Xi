using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	// TODO Somehow move this into its own namespace
	internal partial class Compiler
	{
		private TokenStream stream;
 
		public Compiler()
		{
			Classes = new List<Class>();
		}

		public bool Compile(TokenStream tokenStream)
		{
			stream = tokenStream;

			try
			{
				Program();
			}
			catch(StackOverflowException e)
			{
				Console.WriteLine(e.Message);
				return false;
			}

			return true;
		}

		public void DumpInstructionStream()
		{
			foreach (Instruction instr in Instructions)
			{
				Console.Write("{0}\t", instr.Opcode);

				if (instr.Operands != null)
				{
					foreach (Variant v in instr.Operands)
						Console.Write("{0} ", v);
				}

				Console.WriteLine("");
			}
		}
	}
}
