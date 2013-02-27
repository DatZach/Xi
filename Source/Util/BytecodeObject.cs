using System.Collections.Generic;
using System.IO;
using Xi.Compile;
using Xi.Vm;

namespace Xi.Util
{
	public class BytecodeObject
	{
		private const uint MagicNumber = 0x5842434F;
		private const ushort Version = 1;

		public List<Module> Modules { get; private set; }

		public BytecodeObject()
		{
			Modules = new List<Module>();
		}

		public BytecodeObject(Compiler compiler)
		{
			Modules = compiler.Modules;
		}

		public void Save(string filename)
		{
			using (BinaryWriter stream = new BinaryWriter(new FileStream(filename, FileMode.Create)))
			{
				// Header
				stream.Write(MagicNumber);
				stream.Write(Version);
				stream.Write(Compiler.Version);
				stream.Write(VirtualMachine.Version);

				// Body
				stream.Write(Modules.Count);
				foreach (Module module in Modules)
					WriteModule(module, stream);
			}
		}

		public void Load(string filename)
		{

		}

		private static void WriteModule(Module module, BinaryWriter stream)
		{
			stream.Write(module.Name);
			stream.Write((ushort)module.Fields.Count);

			WriteMethod(module.Body, stream);

			stream.Write(module.Classes.Count);
			foreach (Class cClass in module.Classes)
				WriteClass(cClass, stream);

			stream.Write(module.Methods.Count);
			foreach(Method method in module.Methods)
				WriteMethod(method, stream);
		}

		private static void WriteClass(Class cClass, BinaryWriter stream)
		{
			stream.Write(cClass.Name);
			stream.Write(cClass.Base.Name);
			stream.Write((ushort)cClass.Fields.Count);

			stream.Write(cClass.Methods.Count);
			foreach (Method method in cClass.Methods)
				WriteMethod(method, stream);
		}

		private static void WriteMethod(Method method, BinaryWriter stream)
		{
			stream.Write(method.Name);
			stream.Write((ushort)method.ArgumentCount);
			stream.Write((ushort)method.Variables.Count);

			// TODO This is incredibly lazy and it's bulkier than Nick's Mom because of it
			stream.Write((uint)method.Instructions.Count);
			foreach (Instruction instr in method.Instructions)
			{
				stream.Write((byte)instr.Opcode);
				if (instr.Operands == null)
					continue;

				stream.Write((byte)instr.Operands.Count);
				foreach(Variant v in instr.Operands)
				{
					stream.Write((byte)v.Type);

					switch(v.Type)
					{
						case VariantType.String:
							stream.Write(v.StringValue);
							break;

						case VariantType.Double:
							stream.Write(v.DoubleValue);
							break;

						case VariantType.Int64:
							stream.Write(v.IntValue);
							break;

						case VariantType.Array:
							stream.Write((uint)v.ArrayValue.Count);
							break;

						case VariantType.Object:
							// TODO What to do?
							break;

						case VariantType.Nil:
							break;
					}
				}
			}
		}
	}
}
