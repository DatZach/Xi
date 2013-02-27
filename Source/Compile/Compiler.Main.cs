using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		public const ushort Version = 1;

		private TokenStream stream;
 
		public Compiler()
		{
			Modules = new List<Module>();
			CurrentMethod = null;
		}

		public bool Compile(TokenStream tokenStream)
		{
			stream = tokenStream;

			try
			{
				Module(Vm.Module.GetNameFromFilename(tokenStream.Filename));
			}
			catch (CompilerException e)
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
