﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	class Compiler
	{
		public List<Instruction> Instructions { get; private set; }
 
		public Compiler()
		{
			Instructions = new List<Instruction>();
		}

		public void Compile(TokenStream tokenStream)
		{
			Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
			Instructions.Add(new Instruction(Opcode.Push, new Variant(2)));
			Instructions.Add(new Instruction(Opcode.Add));
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
