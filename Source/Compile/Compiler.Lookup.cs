using System;
using System.Collections.Generic;
using System.Linq;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		public const string ClassNameDefault = "Global";
		public const string MethodNameEntry = "Main";

		public List<Module> Modules { get; private set; }
		private readonly Stack<Module> moduleStack; 
		private readonly Stack<Method> methodStack; 

		private Module CurrentModule
		{
			get
			{
				return moduleStack.Peek();
				//return Modules.Count != 0 ? Modules.Last() : null;
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
				return methodStack.Peek();
			}
		}

		public List<Instruction> Instructions
		{
			get
			{
				if (CurrentMethod == null)
				{
					Stream.Error("Cannot declare body outside of method.");
					return null;
				}

				return CurrentMethod.Instructions;
			}
		}

		void AddModule(string name)
		{
			foreach (Module m in Modules)
				if (m.Name == name)
					Stream.Error("Module \"{0}\" already declared previously.", m.Name);

			Module module = new Module(name);
			moduleStack.Push(module);
			Modules.Add(module);
		}

		// TODO Might want to replace this wherever used
		void LeaveModule()
		{
			moduleStack.Pop();
		}

		void AddModuleBody()
		{
			// Neither of these errors should technically be triggerable
			if (CurrentModule == null)
			{
				Stream.Error("Cannot declare body to non-existant module.");
				return;
			}

			if (CurrentModule.Body != null)
			{
				Stream.Error("Module \"{0}\" already has a body declared!", CurrentModule.Name);
				return;
			}

			CurrentModule.Body = new Method("Body", 0);
			//CurrentMethod = CurrentModule.Body;
			methodStack.Push(CurrentModule.Body);
		}

		void AddClass(string name, Class cBase = null)
		{
			foreach (Class c in CurrentModule.Classes.Where(c => c.Name == name))
				Stream.Error("Class \"{0}\" already declared previously.", c.Name);

			CurrentModule.Classes.Add(new Class(name, cBase));
		}

		void AddMethod(string name, int argCount = 0)
		{
			if (CurrentClass == null)
			{
				foreach (Method m in CurrentModule.Methods.Where(m => m.Name == name))
					Stream.Error("Method \"{0}\" already declared previously.", m.Name);

				//CurrentMethod = new Method(name, argCount);
				methodStack.Push(new Method(name, argCount));
				CurrentModule.Methods.Add(CurrentMethod);
			}
			else
			{

				foreach (Method m in CurrentClass.Methods.Where(m => m.Name == name))
					Stream.Error("Method \"{0}\" already declared previously.", m.Name);

				//CurrentMethod = new Method(name, argCount);
				methodStack.Push(new Method(name, argCount));
				CurrentClass.Methods.Add(CurrentMethod);
			}
		}

		// TODO Maybe just have methodStack.Pop() wherever this is needed?
		void LeaveMethod()
		{
			methodStack.Pop();
		}

		void AddVariable(string name)
		{
			if (CurrentMethod == null)
			{
				Stream.Error("Cannot declare variable outside of method scope.");
				return;
			}

			CurrentMethod.Variables.Add(name);
		}

		int AddTempVariable()
		{
			Random rng = new Random();
			string name;

			if (CurrentMethod == null)
			{
				Stream.Error("Cannot create temporary variable");
				return -1;
			}

			do
			{
				name = "__tempVariable" + rng.Next().ToString("G");
			} while (CurrentMethod.Variables.IndexOf(name) != -1);

			AddVariable(name);

			return GetVariableIndex(name);
		}

		int GetVariableIndex(string name)
		{
			if (CurrentMethod == null)
			{
				Stream.Error("Cannot get variable outside of method scope.");
				return 0;
			}

			int index = CurrentMethod.Variables.IndexOf(name);
			if (index == -1)
				Stream.Error("Undeclared variable \"{0}\".", name);

			return index;
		}

		bool IsVariable(string name)
		{
			if (CurrentMethod == null)
				return false;

			return CurrentMethod.Variables.IndexOf(name) != -1;
		}

		void AddField(string name, Variant initializer = null)
		{
			if (CurrentClass == null)
			{
				Stream.Error("Cannot add field outside of class!");
				return;
			}

			Variant v = initializer ?? new Variant();

			CurrentClass.FieldsCompiler.Add(name, v);
			CurrentClass.Fields.Add(v);
		}
	}
}
