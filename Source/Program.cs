using System;
using System.Text;
using System.Collections.Generic;
using Xi.Util;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		public static void Main(string[] args)
		{
			State state;
			List<Class> program;
			VirtualMachine virtualMachine = new VirtualMachine();

			if (args.Length == 1 && args[0] == "--help")
			{
				Console.WriteLine("Usage: Xi [filename] [entrypoint]");
				return;
			}

			if (args.Length != 0)
			{
				program = Assembler.AssembleFile(args[0]);
			}
			else
			{
				StringBuilder text = new StringBuilder();

				while (true)
				{
					string line = Console.ReadLine();
					if (line == "")
					{
						break;
					}

					text.AppendLine(line);
				}

				program = Assembler.AssembleString(text.ToString());
			}

			if (program == null)
			{
				Console.WriteLine("[Traceback] Cannot run program, syntax error encountered.");
				return;
			}

			virtualMachine.Classes.AddRange(program);

			if (args.Length > 1)
			{
				state = virtualMachine.CreateState("Global", args[1]);
			}
			else
			{
				state = virtualMachine.CreateState(0, 0);
			}

			state.Scope.Foo = new Variant("Hello, World!");

			virtualMachine.Execute(state);
		}
	}
}
