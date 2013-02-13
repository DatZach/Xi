using System.Collections.Generic;
using System.Linq;
using Xi.Vm;

namespace Xi
{
	internal partial class Compiler
	{
		public const string ClassNameDefault = "Global";
		public const string MethodNameEntry = "Main";

		public List<Module> Modules { get; private set; }
		//public List<Class> Classes { get; private set; } // TODO remove

		private Module CurrentModule
		{
			get
			{
				return Modules.Count != 0 ? Modules.Last() : null;
			}
		}

		private Class CurrentClass
		{
			get
			{
				return CurrentModule.Classes.Count != 0 ? CurrentModule.Classes.Last() : null;
			}
		}

		private Method CurrentMethod
		{
			get
			{
				return CurrentModule.Body;

				// TODO Fix this
				//return CurrentClass != null ? CurrentClass.Methods.Last() : null;
			}
		}

		public List<Instruction> Instructions
		{
			get
			{
				if (CurrentMethod == null)
				{
					Error("Cannot declare body outside of method.");
					return null;
				}

				return CurrentMethod.Instructions;
			}
		}

		void AddModule(string name)
		{
			foreach (Module m in Modules)
				if (m.Name == name)
					Error("Module \"{0}\" already declared previously.", m.Name);

			Modules.Add(new Module(name));
		}

		void AddModuleBody()
		{
			// Neither of these errors should technically be triggerable
			if (CurrentModule == null)
			{
				Error("Cannot declare body to non-existant module.");
				return;
			}

			if (CurrentModule.Body != null)
			{
				Error("Module \"{0}\" already has a body declared!", CurrentModule.Name);
				return;
			}

			CurrentModule.Body = new Method("Body", 0);
		}

		void AddClass(string name, Class cBase = null)
		{
			foreach (Class c in CurrentModule.Classes.Where(c => c.Name == name))
				Error("Class \"{0}\" already declared previously.", c.Name);

			CurrentModule.Classes.Add(new Class(name, cBase));
		}

		void AddMethod(string name, int argCount = 0)
		{
			if (CurrentClass == null)
			{
				Error("Cannot declare method outside of class scope.");
				return;
			}

			foreach (Method m in CurrentClass.Methods.Where(m => m.Name == name))
				Error("Method \"{0}\" already declared previously.", m.Name);

			CurrentClass.Methods.Add(new Method(name, argCount));
		}

		void AddVariable(string name)
		{
			if (CurrentMethod == null)
			{
				Error("Cannot declare variable outside of method scope.");
				return;
			}

			CurrentMethod.Variables.Add(name);
		}

		int GetVariableIndex(string name)
		{
			if (CurrentMethod == null)
			{
				Error("Cannot get variable outside of method scope.");
				return 0;
			}

			int index = CurrentMethod.Variables.IndexOf(name);
			if (index == -1)
				Error("Unknown variable \"{0}\".", name);

			return index;
		}
	}
}
