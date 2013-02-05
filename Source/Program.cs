using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 1)
			{
				switch(args[0])
				{
					case "--help":
						Console.WriteLine("Usage: Xi [filename] [entrypoint]");
						return;

					case "--version":
						Console.WriteLine("Xi Version 1.idkman");
						return;
				}
			}

			if (args.Length != 0)
				RunProgram(args[0], args.Length > 1 ? args[1] : "");
			else
				RunRepl();

			Console.ReadKey();
		}

		private static void RunProgram(string filename, string entry)
		{
			// TODO Too ugly?
			Script script = new Script();
			try
			{
				if (!script.LoadFile(filename))
				{
					Console.WriteLine("Cannot open script \"{0}\"!", filename);
					return;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			// TODO Fix this bs
			if (String.IsNullOrEmpty(entry))
				entry = "Global.Main";

			script.Call(entry);
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
