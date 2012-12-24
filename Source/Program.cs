using System;
using System.Collections.Generic;
using Xi.Util;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		// xi [function] <script> [command line arguments]
		public static void Main(string[] args)
		{
			List<Class> program;

			if (args.Length == 0)
			{
				// Operate in live scripting mode
				string line, text = "";

				while ((line = Console.ReadLine()) != "")
					text += line + "\n";

				program = Assembler.AssembleString(text);
			}
			else
				program = Assembler.AssembleFile(args[args.Length == 1 ? 0 : 1]);

			if (program == null)
			{
				Console.WriteLine("[Traceback] Cannot run program, syntax error encountered.");
				return;
			}

			VirtualMachine vm = new VirtualMachine();
			vm.Classes.AddRange(program);

			State state = args.Length == 2 ? vm.CreateState("Global", args[0]) : vm.CreateState(0, 0);

			vm.Execute(state);
		}
	}
}
