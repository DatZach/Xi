using System;
using System.Collections.Generic;
using System.IO;
using Xi.Compile;
using Xi.Vm;

namespace Xi.Util
{
	public class BytecodeObject
	{
		private const uint MagicNumber = 0x4F434258;
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
			using (BinaryReader stream = new BinaryReader(new FileStream(filename, FileMode.Open)))
			{
				// Header
				if (stream.ReadUInt32() != MagicNumber)
					throw new Exception("Bad magic number");

				ushort version = stream.ReadUInt16();
				if (version != Version)
					throw new Exception(String.Format("Object version is {0}, reader only supports version {1}", version, Version));

				stream.ReadUInt16();

				version = stream.ReadUInt16();
				if (version != VirtualMachine.Version)
					throw new Exception(String.Format("Object VM version is {0}, VM is version {1}", version, VirtualMachine.Version));

				int count = stream.ReadInt32();
				while (count-- != 0)
					Modules.Add(ReadModule(stream));
			}
		}

		private void WriteModule(Module module, BinaryWriter stream)
		{
			stream.Write(module.Name);
			stream.Write((ushort)module.Fields.Count);

			WriteMethod(module.Body, stream);

			stream.Write(module.Classes.Count);
			foreach (Class cClass in module.Classes)
				WriteClass(cClass, stream);

			stream.Write(module.Methods.Count);
			foreach (Method method in module.Methods)
				WriteMethod(method, stream);
		}

		private void WriteClass(Class cClass, BinaryWriter stream)
		{
			stream.Write(cClass.Name);
			stream.Write(cClass.Base.Name);
			stream.Write((ushort)cClass.Fields.Count);

			stream.Write(cClass.Methods.Count);
			foreach (Method method in cClass.Methods)
				WriteMethod(method, stream);
		}

		private void WriteMethod(Method method, BinaryWriter stream)
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
				{
					stream.Write((byte)0);
					continue;
				}

				stream.Write((byte)instr.Operands.Count);
				foreach (Variant v in instr.Operands)
				{
					stream.Write((byte)v.Type);

					switch (v.Type)
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

		private Module ReadModule(BinaryReader stream)
		{
			string name = stream.ReadString();
			Module module = new Module(name);

			ushort fieldCount = stream.ReadUInt16();
			for (ushort i = 0; i < fieldCount; ++i)
				module.Fields.Add(new Variant());

			module.Body = ReadMethod(stream);

			uint classCount = stream.ReadUInt32();
			while(classCount-- != 0)
				module.Classes.Add(ReadClass(stream));

			uint methodCount = stream.ReadUInt32();
			while(methodCount-- != 0)
				module.Methods.Add(ReadMethod(stream));

			return module;
		}

		private Class ReadClass(BinaryReader stream)
		{
			List<Variant> fields;
			string name = stream.ReadString();
			string baseName = stream.ReadString();

			Class cClass = new Class(name, null);

			ushort fieldCount = stream.ReadUInt16();
			while(fieldCount-- != 0)
				cClass.Fields.Add(new Variant());

			uint methodCount = stream.ReadUInt32();
			while(methodCount-- != 0)
				cClass.Methods.Add(ReadMethod(stream));

			return cClass;
		}

		private Method ReadMethod(BinaryReader stream)
		{
			List<Instruction> instructions = new List<Instruction>();
			List<string> variables = new List<string>();

			string name = stream.ReadString();
			ushort argumentCount = stream.ReadUInt16();
			ushort variableCount = stream.ReadUInt16();
			while(variableCount-- != 0)
				variables.Add("idkmybffjill");

			uint length = stream.ReadUInt32();
			while(length-- != 0)
			{
				List<Variant> operands = new List<Variant>();
				Opcode opcode = (Opcode)stream.ReadByte();
				
				byte operandCount = stream.ReadByte();
				while(operandCount-- != 0)
				{
					VariantType type = (VariantType)stream.ReadByte();
					switch (type)
					{
						case VariantType.String:
							operands.Add(new Variant(stream.ReadString()));
							break;

						case VariantType.Double:
							operands.Add(new Variant(stream.ReadDouble()));
							break;

						case VariantType.Int64:
							operands.Add(new Variant(stream.ReadInt64()));
							break;

						case VariantType.Object:
							break;

						case VariantType.Array:
						{
							List<Variant> arrayValue = new List<Variant>();
							uint aCount = stream.ReadUInt32();
							while(aCount-- != 0)
								arrayValue.Add(new Variant());

							operands.Add(new Variant(arrayValue));
							break;
						}

						case VariantType.Nil:
							operands.Add(new Variant());
							break;
					}
				}

				instructions.Add(new Instruction(opcode, operands));
			}

			return new Method(name, instructions, argumentCount) { Variables = variables };
		}
	}
}
