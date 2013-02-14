using System.Collections.Generic;
using System.Linq;

namespace Xi.Compile
{
	class Label
	{
		private readonly Compiler compiler;
		private readonly List<int> patches; 

		public Label(Compiler compiler)
		{
			patches = new List<int>();
			this.compiler = compiler;
		}

		public void Mark()
		{
			int labelOffset = compiler.Instructions.Count;

			foreach(int patchOffset in patches)
				compiler.Instructions[patchOffset].Operand.IntValue = labelOffset;
		}

		public void PatchHere()
		{
			compiler.Instructions.Last().Operand.IntValue = 0;
			patches.Add(compiler.Instructions.Count - 1);
		}
	}
}
