using System.Collections.Generic;
using System.Linq;
using Xi.Vm;

namespace Xi.Compile
{
	internal partial class Compiler
	{
		public const string ClassNameDefault = "Global";
		public const string MethodNameEntry = "Main";

		public List<Module> Modules { get; private set; }

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

		private Method CurrentMethod { get; set; }

		public List<Instruction> Instructions
		{
			get
			{
				if (CurrentMethod == null)
				{
					stream.Error("Cannot declare body outside of method.");
					return null;
				}

				return CurrentMethod.Instructions;
			}
		}

		void AddModule(string name)
		{
			foreach (Module m in Modules)
				if (m.Name == name)
					stream.Error("Module \"{0}\" already declared previously.", m.Name);

			Modules.Add(new Module(name));
		}

		void AddModuleBody()
		{
			// Neither of these errors should technically be triggerable
			if (CurrentModule == null)
			{
				stream.Error("Cannot declare body to non-existant module.");
				return;
			}

			if (CurrentModule.Body != null)
			{
				stream.Error("Module \"{0}\" already has a body declared!", CurrentModule.Name);
				return;
			}

			CurrentModule.Body = new Method("Body", 0);
			CurrentMethod = CurrentModule.Body;
		}

		void AddClass(string name, Class cBase = null)
		{
			foreach (Class c in CurrentModule.Classes.Where(c => c.Name == name))
				stream.Error("Class \"{0}\" already declared previously.", c.Name);

			CurrentModule.Classes.Add(new Class(name, cBase));
		}

		void AddMethod(string name, int argCount = 0)
		{
			if (CurrentClass == null)
			{
				foreach (Method m in CurrentModule.Methods.Where(m => m.Name == name))
					stream.Error("Method \"{0}\" already declared previously.", m.Name);

				CurrentMethod = new Method(name, argCount);
				CurrentModule.Methods.Add(CurrentMethod);
			}
			else
			{

				foreach (Method m in CurrentClass.Methods.Where(m => m.Name == name))
					stream.Error("Method \"{0}\" already declared previously.", m.Name);

				CurrentMethod = new Method(name, argCount);
				CurrentClass.Methods.Add(CurrentMethod);
			}
		}

		void AddVariable(string name)
		{
			if (CurrentMethod == null)
			{
				stream.Error("Cannot declare variable outside of method scope.");
				return;
			}

			CurrentMethod.Variables.Add(name);
		}

		int GetVariableIndex(string name)
		{
			if (CurrentMethod == null)
			{
				stream.Error("Cannot get variable outside of method scope.");
				return 0;
			}

			int index = CurrentMethod.Variables.IndexOf(name);
			if (index == -1)
				stream.Error("Undeclared variable \"{0}\".", name);

			return index;
		}

		bool IsVariable(string name)
		{
			if (CurrentMethod == null)
				return false;

			return CurrentMethod.Variables.IndexOf(name) != -1;
		}
	}
}
