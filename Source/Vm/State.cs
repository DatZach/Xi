using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Xi.Vm
{
	class State
	{
		public List<Module> Modules { get; private set; }
		public VmStack<CallInfo> CallStack;
		public VmStack<Variant> Stack;
		public dynamic Scope;

		//public CallInfo CurrentCall;

		public Module CurrentModule
		{
			get { return CallStack.Top.Module; }
		}

		public Class CurrentClass
		{
			get
			{
				return CallStack.Top.Class;
			}
		}

		public Method CurrentMethod
		{
			get
			{
				return CallStack.Top.Method;
			}

			// TODO wth is this?
			/*set
			{
				if (CurrentClass == null)
					return;

				CurrentClass.Methods[CallStack.Top.MethodIndex] = value;
			}*/
		}

		public int InstructionPointer
		{
			get { return CallStack.Top.InstructionPointer; }
			set { CallStack.Top.InstructionPointer = value; }
		}

		public State()
		{
			Modules = new List<Module>();
			CallStack = new VmStack<CallInfo>();
			Stack = new VmStack<Variant>();
			InstructionPointer = 0;
			Scope = new ExpandoObject();
		}

		public void SetEntryPoint(string moduleName)
		{
			// Find module
			Module module = Modules.First(m => m.Name == moduleName);
			if (module == null)
				throw new Exception(String.Format("Cannot set entry point to non-existant module \"{0}\".", moduleName));

			// Make sure it has a body
			if (module.Body == null)
				throw new Exception(String.Format("Module \"{0}\" has no body, cannot set entry point here.", moduleName));

			// TODO This is a hack, just get rid of indexs, they're silly
			CallStack.Push(new CallInfo(module, -1, 0));
		}

		/*public void SetEntryPoint(string className, string methodName)
		{
			Class classHandle = Classes.FirstOrDefault(c => c.Name == className);
			if (classHandle == null)
				throw new Exception(String.Format("No class \"{0}\" exists.", className));

			int methodIndex = classHandle.GetMethodIndex(methodName);
			if (methodIndex == -1)
				throw new Exception(String.Format("No method \"{0}\" is a member of class \"{1}\".", methodName, className));

			CurrentCall = new CallInfo(classHandle, methodIndex, 0);
		}*/
	}
}
