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
			List<Class> program;

			if (args.Length == 1 && args[0] == "--help")
			{
				Console.WriteLine("Usage: Xi [filename] [entrypoint]");
				return;
			}

			if (args.Length != 0)
				RunProgram(args[0], args.Length > 1 ? args[1] : "");
			else
				RunRepl();

			Console.ReadKey();
		}

		private static void RunProgram(string filename, string entry)
		{
			Compiler compiler = new Compiler();

		}

		private static void RunRepl()
		{
			string value;

			do
			{
				Console.Write("> ");
				value = Console.ReadLine();

				Compiler compiler = new Compiler();
				compiler.Compile(new TokenStream(Parser.ParseString(value)));

				// TODO: Preserve VM state
				State state = new State();
				Method entryPointMethod = new Method("Main", compiler.Instructions, 0, 0);
				Class globalClass = new Class("Global", new List<Method> { entryPointMethod }, new List<Variant>(), null);
				state.Classes.Add(globalClass);

				state.SetEntryPoint("Global", "Main");

				VirtualMachine.Execute(state);
			} while (value != "exit");
		}
	}
}
