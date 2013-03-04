using System;
using System.Collections.Generic;
using Xi.Compile;
using Xi.Lexer;
using Xi.Util;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		public static void Main(string[] args)
		{
			// Handle command line
			CommandLine.ParseArguments(args);
			if (CommandLine.Flags.Contains("--help"))
				CommandLine.PrintUsage();
			else if (CommandLine.Flags.Contains("--version"))
				CommandLine.PrintVersion();

			// Run script mode
			if (CommandLine.Filename != null)
				RunScript(CommandLine.Filename);
			else
				RunRepl();

			Console.ReadKey();
		}

		private static void RunScript(string filename)
		{
			// Compile script
			Compiler compiler = new Compiler();

			List<Token> tokenStream = Tokenizer.ParseFile(filename);
			if (tokenStream == null)
				return;

			if (!compiler.Compile(new TokenStream(tokenStream)))
				return;

			//Disassembler.Dump(compiler.Modules);

			// Create state and add modules
			State state = new State();
			state.Modules.AddRange(compiler.Modules);

			// Run script
			try
			{
				state.SetEntryPoint(Module.GetNameFromFilename(filename));
				VirtualMachine.Execute(state);
			}
			catch (StackOverflowException e)
			{
				Console.WriteLine("VM Error: {0}", e.Message);
			}
		}

		private static void RunRepl()
		{
			string value;

			do
			{
				Console.Write(">> ");
				value = Console.ReadLine();

				// Compile line
				Compiler compiler = new Compiler();

				List<Token> tokenStream = Tokenizer.ParseString(value);
				if (tokenStream == null)
					continue;

				if (!compiler.Compile(new TokenStream(tokenStream)))
					continue;

				Disassembler.Dump(compiler.Modules);

				// Add line
				State state = new State();
				state.Modules.AddRange(compiler.Modules);

				// Run line
				try
				{
					state.SetEntryPoint(Module.ModuleDefaultName);
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
