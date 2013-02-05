using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	internal partial class Compiler
	{
		public List<Instruction> Instructions { get; private set; }
		private TokenStream stream;
 
		public Compiler()
		{
			Instructions = new List<Instruction>();
		}

		public void Compile(TokenStream tokenStream)
		{
			stream = tokenStream;

			try
			{
				Program();
			}
			catch(StackOverflowException e)
			{
				Console.WriteLine(e.Message);
			}
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
