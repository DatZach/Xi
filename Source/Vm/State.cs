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
			Scope = new ExpandoObject();
		}

		public void SetEntryPoint(string moduleName, string methodName = "")
		{
			// Find module
			Module module = Modules.First(m => m.Name == moduleName);
			if (module == null)
				throw new Exception(String.Format("Cannot set entry point to non-existant module \"{0}\".", moduleName));

			// Make sure it has a body if no method is specified
			Method method = module.GetMethod(methodName);
			if (method == null && module.Body == null)
				throw new Exception(String.Format("Module \"{0}\" has no body, cannot set entry point here.", moduleName));
			
			if (method == null)
				method = module.Body;

			CallStack.Push(new CallInfo(module, method, 0));
		}
	}
}
