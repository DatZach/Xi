using System.Collections.Generic;
using System.Linq;

namespace Xi.Vm
{
	class Module
	{
		public List<Class> Classes { get; private set; }
		public List<Method> Methods { get; private set; } 
		public List<Variant> Fields { get; private set; }
		public Method Body;
		public string Name { get; private set; }

		public Module(string name)
		{
			Classes = new List<Class>();
			Methods = new List<Method>();
			Fields = new List<Variant>();
			Body = null;
			Name = name;
		}

		public Class GetClass(string name)
		{
			return Classes.First(c => c.Name == name);
		}

		public Method GetMethod(string name)
		{
			return Methods.First(m => m.Name == name);
		}

		public int GetMethodIndex(string name)
		{
			return Methods.IndexOf(Methods.Single(m => m.Name == name));
		}
	}
}
