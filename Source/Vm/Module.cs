﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xi.Vm
{
	public class Module
	{
		public const string ModuleDefaultName = "Main";

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
			try
			{
				return Classes.Count == 0 ? null : Classes.First(c => c.Name == name);
			}
			catch
			{
				return null;
			}
		}

		public Method GetMethod(string name)
		{
			try
			{
				if (Body != null && Body.Name == name)
					return Body;

				return Methods.Count == 0 ? null : Methods.First(m => m.Name == name);
			}
			catch
			{
				return null;
			}
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

		public static string GetNameFromFilename(string filename)
		{
			return String.IsNullOrEmpty(filename) ? ModuleDefaultName : Path.GetFileNameWithoutExtension(filename);
		}
	}
}
