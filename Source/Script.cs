using System;
using System.Collections.Generic;
using System.IO;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	public class Script
	{
		private readonly State state;

		public Script()
		{
			state = new State();
		}

		public bool LoadString(string value)
		{
			TokenStream tokenStream = new TokenStream(Tokenizer.ParseString(value));

			Compiler compiler = new Compiler();
			if (!compiler.Compile(tokenStream))
				return false;

			// TODO Read classes/methods from compiler instead of implicitly declaring them here
			//Method entryPointMethod = new Method("Main", compiler.Instructions, 0, 0);
			//Class globalClass = new Class("Global", new List<Method> { entryPointMethod }, new List<Variant>(), null);
			state.Classes.AddRange(compiler.Classes);

			return true;
		}

		public bool LoadFile(string filename)
		{
			try
			{
				using (TextReader reader = new StreamReader(filename))
				{
					string value = reader.ReadToEnd();
					return LoadString(value);
				}
			}
			catch(IOException)
			{
				return false;
			}
		}

		public Variant[] Call(string method, params Variant[] arguments)
		{
			string[ ] values = method.Split('.');
			if (values.Length != 2)
				throw new ArgumentException("Invalid \"method\" value, expected format \"Class.Method\".");

			state.SetEntryPoint(values[0], values[1]);
			VirtualMachine.Execute(state);

			return null;
		}
	}
}
