using System;
using System.Collections.Generic;
using System.Linq;

namespace Xi.Vm
{
	public class Class
	{
		public Module Module;
		public List<Method> Methods { get; private set; }
		public List<Variant> Fields { get; private set; }
		public Class Base { get; private set; }
		public string Name { get; private set; }

		internal Class(string name, Class cBase)
		{
			Module = null;
			Methods = new List<Method>();
			Fields = new List<Variant>();
			Base = cBase;
			Name = name;
		}

		public Class(Class classHandle)
		{
			Module = null;
			Name = classHandle.Name;
			Methods = classHandle.Methods;
			Fields = classHandle.Fields;
			Base = classHandle.Base;
		}

		public Class(string name, List<Method> methods, List<Variant> fields, Class cBase)
		{
			Module = null;
			Name = name;
			Methods = methods;
			Fields = fields;
			Base = cBase;
		}

		public Method GetMethod(string name)
		{
			return Methods.First(m => m.Name == name);
		}

		public int GetMethodIndex(string name)
		{
			try
			{
				return Methods.IndexOf(Methods.First(m => m.Name == name));
			}
			catch (Exception)
			{
				return -1;
			}
		}
	}
}
