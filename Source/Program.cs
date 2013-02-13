using System;
using System.Collections.Generic;
using System.Reflection;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 1 && args[0][0] == '-' && args[0][1] == '-') // TODO Fix this hack
			{
				HandleCommandLineSwitches(args[0]);
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

				// TODO Maybe move the catch up a level to provide better management up here?
				if (!compiler.Compile(new TokenStream(tokenStream)))
					continue;

				Disassembler.Dump(compiler.Modules);

				// TODO: Preserve VM state
				State state = new State();
				state.Modules.AddRange(compiler.Modules);

				try
				{
					// TODO Add Compiler.EntryClass/Compiler.EntryMethod
					//state.SetEntryPoint(Compiler.ClassNameDefault, Compiler.MethodNameEntry);
					// TODO a tad bit of a hack
					state.SetEntryPoint(state.Modules[0].Name);
					VirtualMachine.Execute(state);
				}
				catch (Exception e)
				{
					Console.WriteLine("VM Error: {0}", e.Message);
				}
			} while (value != "exit");
		}

		private static void HandleCommandLineSwitches(string value)
		{
			const int helpTopicTabLength = 14;

			Dictionary<string, string> helpTopics = new Dictionary<string, string>
			{
				{ "[filename]",		"Filename of script to run." },
				{ "[entrypoint]",	"Entry point to start executing the script from.\n" +
									"Follows the format Class.Method, class and method must be static."},
				{ "[flags]",		"Flags that change the behavior of the script." },
				{ "--version",		"Display Xi version." },
				{ "--help",			"Display this information." },
				{ "--args",			"Command line arguments to pass" }
			};

			switch (value)
			{
				case "--help":
					Console.WriteLine("Usage: Xi [flags] [filename] [entrypoint]");
					foreach(KeyValuePair<string, string> topic in helpTopics)
					{
						int indentValue = topic.Key.Length;
						Console.Write(topic.Key);

						foreach (string helpValue in topic.Value.Split(new [] { '\n' }))
						{
							for (; indentValue < helpTopicTabLength; ++indentValue)
								Console.Write(" ");

							Console.WriteLine(helpValue);
							indentValue = 0;
						}
					}
					break;

				case "--version":
					Console.WriteLine("Xi {0}", Assembly.GetExecutingAssembly().GetName().Version);
					break;
			}
		}
	}
}
