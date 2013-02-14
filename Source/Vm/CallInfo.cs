namespace Xi.Vm
{
	/*
	 *	CallInfo should be context sensitive, meaning context sensitive instructions from the VM should
	 *		be able to use the CallInfo on top of the Call Stack to determine context without the aid of anything else.
	 *	
	 *	Calls from classes will always provide the Class field.
	 *	Calls from modules will NEVER provide the Class field.
	 *	
	 *	Module			Handle to module (CANNOT BE NULL)
	 *	Class			Handle to call
	 *					IF NULL				Call came from Module
	 *					ELSE				Call came from Class
	 *	MethodIndex		Index of method		CONTEXT SENSITIVE
	 *	InstrPtr		Instruction Pointer
	 */

	class CallInfo
	{
		public Module Module { get; private set; }
		public Class Class { get; private set; }
		public int InstructionPointer;

		public Method Method { get; private set; }

		public CallInfo()
		{
			Module = null;
			Class = null;
			Method = null;
			InstructionPointer = 0;
		}

		public CallInfo(Module vmModule, Class vmClass, Method method, int instructionPointer)
		{
			Module = vmModule;
			Class = vmClass;
			Method = method;
			InstructionPointer = instructionPointer;
		}

		public CallInfo(Module vmModule, Method method, int instructionPointer)
		{
			Module = vmModule;
			Class = null;
			Method = method;
			InstructionPointer = instructionPointer;
		}
	}
}
