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
			var a = Compiler.Lexer.Parser.ParseFile("test.xi");

			VirtualMachine virtualMachine = new VirtualMachine();
			List<Class> program;

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
						break;

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

			State state = args.Length > 1 ? virtualMachine.CreateState("Global", args[1]) : virtualMachine.CreateState(0, 0);

			try
			{
				virtualMachine.Execute(state);
			}
			catch (Exception e)
			{
				// Add a stack trace in here eventually
				Console.WriteLine("[Traceback] {0}", e.Message);
			}

			Console.ReadKey();
		}
	}
}
