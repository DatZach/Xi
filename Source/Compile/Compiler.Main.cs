using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		public const ushort Version = 1;

		//private TokenStream stream;
		private readonly Stack<TokenStream> streamStack;
 
		private TokenStream Stream
		{
			get
			{
				return streamStack.Peek();
			}
		}
 
		public Compiler()
		{
			streamStack = new Stack<TokenStream>();
			methodStack = new Stack<Method>();
			Modules = new List<Module>();
		}

		public bool Compile(TokenStream tokenStream)
		{
			streamStack.Push(tokenStream);

			try
			{
				Module(Vm.Module.GetNameFromFilename(tokenStream.Filename));
			}
			catch (CompilerException e)
			{
				streamStack.Pop();
				Console.WriteLine(e.Message);
				return false;
			}

			streamStack.Pop();

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
