using System.Collections.Generic;
using System.Linq;
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
			if (tokens == null)
				return false;

			return CompileStream(new TokenStream(tokens));
		}

		public bool LoadFile(string filename)
		{
			List<Token> tokens = Tokenizer.ParseFile(filename);
			return CompileStream(new TokenStream(tokens));
		}

		public void Call()
		{
			state.SetEntryPoint(state.Modules.First().Name);
			VirtualMachine.Execute(state);
		}

		public Variant[] Call(string method, params Variant[] arguments)
		{
			state.SetEntryPoint(state.Modules.First().Name, method);
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
