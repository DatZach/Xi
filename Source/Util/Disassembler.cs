using System;
using System.Collections.Generic;
using Xi.Vm;

namespace Xi.Util
{
	internal static class Disassembler
	{
		private const int OperandTabLength = 18;

		public static void Dump(List<Module> modules)
		{
			foreach (Module module in modules)
			{
				Console.WriteLine("--- Module {0} ---", module.Name);

				// Dump body if it exists
				if (module.Body != null)
				{
					Console.WriteLine("--- Body ---");
					DumpInstructions(module.Body.Instructions);
				}

				foreach (Method m in module.Methods)
				{
					Console.WriteLine("--- Method {0} : {1} ---", m.Name, m.ArgumentCount);
					DumpInstructions(m.Instructions);
				}

				// Dump all classes
				foreach (Class c in module.Classes)
				{
					Console.WriteLine("--- Class {0} : {1} ---", c.Name, c.Base != null ? c.Base.Name : "null");
					foreach (Method m in c.Methods)
					{
						Console.WriteLine("--- Method {0} : {1} ---", m.Name, m.ArgumentCount);
						DumpInstructions(m.Instructions);
					}
				}
			}
		}

		private static void DumpInstructions(IEnumerable<Instruction> instructions)
		{
			int ii = 0;
			foreach (Instruction instr in instructions)
			{
				Console.Write("{0}\t{1}", ii++, instr.Opcode);
				for (int i = instr.Opcode.ToString().Length; i < OperandTabLength; ++i)
					Console.Write(' ');

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
