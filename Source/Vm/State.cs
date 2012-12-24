using System.Dynamic;

namespace Xi.Vm
{
	class State
	{
		public VmStack<CallInfo> CallStack;
		public VmStack<Variant> Stack;
		public dynamic Scope;

		public CallInfo CurrentCall;

		public Class CurrentClass
		{
			get
			{
				return CurrentCall.Class;
			}
		}

		public Method CurrentMethod
		{
			get
			{
				return CurrentClass == null ? null : CurrentClass.Methods[CurrentCall.MethodIndex];
			}

			set
			{
				if (CurrentClass == null)
					return;

				CurrentClass.Methods[CallStack.Top.MethodIndex] = value;
			}
		}

		public int InstructionPointer
		{
			get { return CurrentCall.InstructionPointer; }
			set { CurrentCall.InstructionPointer = value; }
		}

		public State()
		{
			CurrentCall = new CallInfo();
			CallStack = new VmStack<CallInfo>();
			Stack = new VmStack<Variant>();
			InstructionPointer = 0;
			Scope = new ExpandoObject();
		}
	}
}
