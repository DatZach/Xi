using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Xi.Vm
{
	class State
	{
		public List<Class> Classes { get; private set; }
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
			Classes = new List<Class>();
		}

		public void SetEntryPoint(string className, string methodName)
		{
			Class classHandle = Classes.FirstOrDefault(c => c.Name == className);
			if (classHandle == null)
				throw new Exception(String.Format("No class \"{0}\" exists.", className));

			int methodIndex = classHandle.GetMethodIndex(methodName);
			if (methodIndex == -1)
				throw new Exception(String.Format("No method \"{0}\" is a member of class \"{1}\".", methodName, className));

			CurrentCall = new CallInfo(classHandle, methodIndex, 0);
		}
	}
}
