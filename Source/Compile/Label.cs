using System.Collections.Generic;
using System.Linq;

namespace Xi.Compile
{
	class Label
	{
		private readonly Compiler compiler;
		private readonly List<int> patches;
		private int labelOffset;

		public Label(Compiler compiler)
		{
			patches = new List<int>();
			labelOffset = 0;
			this.compiler = compiler;
		}

		public void Mark()
		{
			labelOffset = compiler.Instructions.Count;
		}

		public void PatchHere()
		{
			compiler.Instructions.Last().Operand.IntValue = 0;
			patches.Add(compiler.Instructions.Count - 1);
		}

		public void Fix()
		{
			foreach (int patchOffset in patches)
				compiler.Instructions[patchOffset].Operand.IntValue = labelOffset;
		}
	}
}
