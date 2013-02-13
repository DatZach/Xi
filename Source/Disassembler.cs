using System;
using System.Collections.Generic;
using Xi.Vm;

namespace Xi
{
	internal static class Disassembler
	{
		public static void Dump(List<Module> modules)
		{
			const int operandTabLength = 18;

			foreach (Module module in modules)
			{
				int ii = 0;

				Console.WriteLine("--- Module {0} ---", module.Name);
				Console.WriteLine("--- Body ---");
				
				if (module.Body != null)
				{
					ii = 0;
					foreach (Instruction instr in module.Body.Instructions)
					{
						Console.Write("{0}\t{1}", ii++, instr.Opcode);
						for (int i = instr.Opcode.ToString().Length; i < operandTabLength; ++i)
							Console.Write(" ");

						if (instr.Operands != null)
						{
							foreach (Variant v in instr.Operands)
								Console.Write("{0} ", v);
						}

						Console.WriteLine("");
					}
				}

				foreach (Class c in module.Classes)
				{
					Console.WriteLine("--- Class {0} : {1} ---", c.Name, c.Base != null ? c.Base.Name : "null");
					foreach (Method m in c.Methods)
					{
						Console.WriteLine("--- Method {0} : {1} ---", m.Name, m.ArgumentCount);
						ii = 0;
						foreach (Instruction instr in m.Instructions)
						{
							Console.Write("{0}\t{1}", ii++, instr.Opcode);
							for (int i = instr.Opcode.ToString().Length; i < operandTabLength; ++i)
								Console.Write(" ");

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
		}
	}
}
