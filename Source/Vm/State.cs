namespace Xi.Vm
{
	class State
	{
		public VmStack<CallInfo> CallStack;
		public VmStack<Variant> Stack;
		public int InstructionPointer;
		public dynamic Scope;

		public Class CurrentClass
		{
			get
			{
				return CallStack.Count == 0 ? null : CallStack.Top.Class;
			}
		}

		public Method CurrentMethod
		{
			get { return CurrentClass == null ? null : CurrentClass.Methods[CallStack.Top.MethodIndex]; }
		}

		public State()
		{
			CallStack = new VmStack<CallInfo>();
			Stack = new VmStack<Variant>();
			InstructionPointer = 0;
			Scope = null;
		}
	}
}
