using System.Collections.Generic;
using System.Linq;
using Xi.Vm;

namespace Xi
{
	internal partial class Compiler
	{
		public const string ClassNameDefault = "Global";
		public const string MethodNameEntry = "Main";

		public List<Class> Classes { get; private set; }

		private Class CurrentClass
		{
			get { return Classes.Count != 0 ? Classes.Last() : null; }
		}

		private Method CurrentMethod
		{
			get { return CurrentClass != null ? CurrentClass.Methods.Last() : null; }
		}

		private List<Instruction> Instructions
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

		void AddClass(string name, Class cBase = null)
		{
			foreach (Class c in Classes.Where(c => c.Name == name))
				Error(string.Format("Class \"{0}\" already declared previously.", c.Name));

			Classes.Add(new Class(name, cBase));
		}

		void AddMethod(string name, int argCount = 0)
		{
			if (CurrentClass == null)
			{
				Error("Cannot declare method outside of class scope.");
				return;
			}

			foreach (Method m in CurrentClass.Methods.Where(m => m.Name == name))
				Error(string.Format("Method \"{0}\" already declared previously.", m.Name));

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

			return CurrentMethod.Variables.IndexOf(name);
		}
	}
}
