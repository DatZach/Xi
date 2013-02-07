using System.Collections.Generic;

namespace Xi.Vm
{
	public class Method
	{
		public List<Instruction> Instructions { get; private set; }
		public int VariableCount { get; private set; }
		public int ArgumentCount { get; private set; }
		public string Name { get; private set; }

		internal Method(string name, int argumentCount)
		{
			Instructions = new List<Instruction>();
			VariableCount = 0;
			ArgumentCount = argumentCount;
			Name = name;
		}

		public Method(string name, List<Instruction> instructions, int variableCount, int argumentCount)
		{
			Name = name;
			Instructions = instructions;
			VariableCount = variableCount;
			ArgumentCount = argumentCount;
		}
	}
}
