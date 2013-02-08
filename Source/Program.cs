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
				entry = Compiler.ClassNameDefault + "." + Compiler.MethodNameEntry;

			script.Call(entry);
		}

		private static void RunRepl()
		{
			string value;

			do
			{
				Console.Write(">> ");
				value = Console.ReadLine();

				List<Token> tokenStream;
				Compiler compiler = new Compiler();

				try
				{
					tokenStream = Tokenizer.ParseString(value);
				}
				catch(Exception e)
				{
					Console.WriteLine("Lexer Error: {0}", e.Message);
					continue;
				}

				compiler.Compile(new TokenStream(tokenStream));
				Disassembler.DumpClasses(compiler.Classes);

				// TODO: Preserve VM state
				State state = new State();
				state.Classes.AddRange(compiler.Classes);

				try
				{
					// TODO Add Compiler.EntryClass/Compiler.EntryMethod
					state.SetEntryPoint(Compiler.ClassNameDefault, Compiler.MethodNameEntry);
					VirtualMachine.Execute(state);
				}
				catch (Exception e)
				{
					Console.WriteLine("VM Error: {0}", e.Message);
				}
			} while (value != "exit");
		}
	}
}
