using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	public class Script
	{
		private State state;

		public bool LoadString(string value)
		{
			TokenStream tokenStream = new TokenStream(Parser.ParseString(value));

			Compiler compiler = new Compiler();
			compiler.Compile(tokenStream);

			return true;
		}

		public bool LoadFile(string filename)
		{
			return true;
		}

		public Variant[] Call(params Variant[] arguments)
		{
			return null;
		}
	}
}
