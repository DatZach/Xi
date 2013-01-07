using System;
using System.Text;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Util;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		public static void Main(string[] args)
		{
			// TODO: This entire method is boched now
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
				string value;

				do
				{
					Compiler compiler = new Compiler();
					Console.Write("> ");
					value = Console.ReadLine();

					compiler.Compile(new TokenStream(Parser.ParseString(value)));

					// TODO: Preserve VM state
					virtualMachine.Classes.Clear();
					virtualMachine.Classes.Add(new Class("Global", new List<Method>() { new Method("Main", compiler.Instructions, 0, 0) }, new List<Variant>(), null));
					virtualMachine.Execute(virtualMachine.CreateState("Global", "Main"));

				} while (value != "exit");
			}

			/*if (program == null)
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
			}*/

			Console.ReadKey();
		}
	}
}
