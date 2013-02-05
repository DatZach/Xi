namespace Xi.Vm
{
	class CallInfo
	{
		public Class Class { get; private set; }
		public int MethodIndex { get; private set; }
		public int InstructionPointer;

		public CallInfo()
		{
			Class = null;
			MethodIndex = 0;
			InstructionPointer = 0;
		}

		public CallInfo(Class vmClass, int methodIndex, int instructionPointer)
		{
			Class = vmClass;
			MethodIndex = methodIndex;
			InstructionPointer = instructionPointer;
		}
	}
}
