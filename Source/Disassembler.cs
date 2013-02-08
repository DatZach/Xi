using System;
using System.Collections.Generic;
using Xi.Vm;

namespace Xi
{
	internal static class Disassembler
	{
		public static void DumpClasses(List<Class> classes)
		{
			foreach(Class c in classes)
			{
				Console.WriteLine("--- Class {0} : {1} ---", c.Name, c.Base != null ? c.Base.Name : "null");
				foreach(Method m in c.Methods)
				{
					Console.WriteLine("--- Method {0} : {1} ---", m.Name, m.ArgumentCount);
					foreach(Instruction instr in m.Instructions)
					{
						Console.Write("{0}", instr.Opcode);
						for (int i = instr.Opcode.ToString().Length; i < 16; ++i)
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
