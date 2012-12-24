namespace Xi.Vm
{
	class CallInfo
	{
		public Class Class { get; private set; }
		public int MethodIndex { get; private set; }
		public int InstructionPointer { get; private set; }

		public CallInfo(Class vmClass, int methodIndex, int instructionPointer)
		{
			Class = vmClass;
			MethodIndex = methodIndex;
			InstructionPointer = instructionPointer;
		}
	}
}
