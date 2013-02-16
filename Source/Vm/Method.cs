using System.Collections.Generic;

namespace Xi.Vm
{
	public class Method
	{
		public List<Instruction> Instructions { get; private set; }
		public List<string> Variables;
		public int ArgumentCount { get; private set; }
		public string Name { get; private set; }

		internal Method(string name, int argumentCount)
		{
			Instructions = new List<Instruction>();
			Variables = new List<string>();
			ArgumentCount = argumentCount;
			Name = name;
		}

		public Method(string name, List<Instruction> instructions, int argumentCount)
		{
			Name = name;
			Instructions = instructions;
			Variables = new List<string>();
			ArgumentCount = argumentCount;
		}
	}
}
