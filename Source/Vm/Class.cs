using System.Collections.Generic;
using System.Linq;

namespace Xi.Vm
{
	class Class
	{
		public List<Method> Methods { get; private set; }
		public List<Variant> Fields { get; private set; }
		public Class Base { get; private set; }
		public string Name { get; private set; }

		public Class(string name, List<Method> methods, List<Variant> fields, Class cBase)
		{
			Name = name;
			Methods = methods;
			Fields = fields;
			Base = cBase;
		}

		public Method GetMethod(string name)
		{
			return Methods.FirstOrDefault(m => m.Name == name);
		}

		public int GetMethodIndex(string name)
		{
			for (int i = 0; i < Methods.Count; ++i)
				if (Methods[i].Name == name)
					return i;

			return -1;
		}
	}
}
