using System;
using System.Collections.Generic;
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
			List<Token> tokens = Tokenizer.ParseString(value);
			return CompileStream(new TokenStream(tokens));
		}

		public bool LoadFile(string filename)
		{
			List<Token> tokens = Tokenizer.ParseFile(filename);
			return CompileStream(new TokenStream(tokens));
		}

		public Variant[] Call(string method, params Variant[] arguments)
		{
			string[ ] values = method.Split('.');
			if (values.Length != 2)
				throw new ArgumentException("Invalid \"method\" value, expected format \"Class.Method\".");

			// TODO a tad bit of a hack
			state.SetEntryPoint(state.Modules[0].Name);
			VirtualMachine.Execute(state);

			return null;
		}

		private bool CompileStream(TokenStream tokenStream)
		{
			Compiler compiler = new Compiler();
			if (!compiler.Compile(tokenStream))
				return false;

			state.Modules.AddRange(compiler.Modules);

			return true;
		}
	}
}
