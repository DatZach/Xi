using System.Collections.Generic;

namespace Xi.Vm
{
	class Method
	{
		public List<Instruction> Instructions { get; private set; }
		public int VariableCount { get; private set; }
		public string Name { get; private set; }

		public Method(string name, List<Instruction> instructions, int variableCount)
		{
			Name = name;
			Instructions = instructions;
			VariableCount = variableCount;
		}
	}
}
