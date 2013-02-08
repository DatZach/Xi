using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xi.Vm;

namespace Xi.Util
{
	class Assembler
	{
		private static string[] stream;
		private static int streamIndex;
		private static ParseLevel parseLevel;

		private static string curClassName, curMethodName;
		private static long curMethodArguments;
		private static List<Class> classes;
		private static List<Method> methods;
		private static List<Variant> fields;
		private static List<Instruction> instructions;

		enum ParseLevel
		{
			Top,
			Class,
			Method
		}

		public static List<Class> AssembleString(string text)
		{
			// Initialize stream
			InitializeStream(text);

			// Begine parsing
			while (streamIndex < stream.Length)
			{
				try
				{
					switch (parseLevel)
					{
						case ParseLevel.Top:
							ParseTopLevel();
							break;

						case ParseLevel.Class:
							ParseClassLevel();
							break;

						case ParseLevel.Method:
							ParseMethodLevel();
							break;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("[Error    ] Failed to assemble file!");
					Console.WriteLine("\t{0}", e.Message);

					return null;
				}
			}

			return classes;
		}

		private static void ParseTopLevel()
		{
			string value = ReadWord();

			if (value != "Class")
				return;

			curClassName = ReadWord();
			methods = new List<Method>();
			fields = new List<Variant>();
			parseLevel = ParseLevel.Class;
		}

		private static void ParseClassLevel()
		{
			switch (ReadWord())
			{
				case "EndClass":
					classes.Add(new Class(curClassName, methods, fields, null));
					parseLevel = ParseLevel.Top;
					break;

				case "Field":
					fields.Add(ReadOperand());
					break;

				case "Method":
					curMethodName = ReadWord();
					curMethodArguments = ReadLong();
					instructions = new List<Instruction>();
					parseLevel = ParseLevel.Method;
					break;
			}
		}

		private static void ParseMethodLevel()
		{
			string value = ReadWord();

			if (value == "EndMethod")
			{
				methods.Add(new Method(curMethodName, instructions, (int)curMethodArguments));
				parseLevel = ParseLevel.Class;
				return;
			}

			Rewind();
			
			Opcode opcode = ReadOpcode();
			if ((byte)opcode == 0xFF)
				throw new Exception(String.Format("Unknown opcode at token {0}", streamIndex));
			
			int operandCount = Instruction.GetOperandCount(opcode);
			if (operandCount == 0)
				instructions.Add(new Instruction(opcode));
			else if (operandCount == 1)
				instructions.Add(new Instruction(opcode, ReadOperand()));
			else
			{
				List<Variant> operands = new List<Variant>();
				for(int i = 0; i < operandCount; ++i)
					operands.Add(ReadOperand());

				instructions.Add(new Instruction(opcode, operands));
			}
		}

		public static List<Class> AssembleFile(string filename)
		{
			using (StreamReader reader = new StreamReader(new FileStream(filename, FileMode.Open)))
			{
				return AssembleString(reader.ReadToEnd());
			}
		}

		private static void InitializeStream(string text)
		{
			text = text.Replace("\r\n", " ");
			text = text.Replace('\n', ' ');
			text = text.Replace("\t", "");
			stream = text.Split(' ').Where(c => c != "").ToArray();

			streamIndex = 0;
			parseLevel = ParseLevel.Top;

			classes = new List<Class>();

			curClassName = "";
			curMethodName = "";
			curMethodArguments = 0;
		}

		private static string ReadWord()
		{
			return stream[streamIndex++];
		}

		private static long ReadLong()
		{
			string value = ReadWord();
			long iValue;

			if (!Int64.TryParse(value, out iValue))
				return 0;

			return iValue;
		}

		private static Opcode ReadOpcode()
		{
			return (Opcode)Enum.GetNames(typeof(Opcode)).ToList().IndexOf(ReadWord());
		}

		private static Variant ReadOperand()
		{
			VariantType type = VariantType.Int64;
			string value = ReadWord();

			if (value.First() == '\"' && value.Last() == '\"')
				type = VariantType.String;
			else if (value.IndexOf('.') != -1)
				type = VariantType.Double;

			switch (type)
			{
				case VariantType.Int64:
					{
						long iValue;
						if (!Int64.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out iValue))
							throw new Exception(String.Format("Cannot parse operand \"{0}\"!", value));

						return new Variant(iValue);
					}

				case VariantType.Double:
					{
						double dValue;
						if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out dValue))
							throw new Exception(String.Format("Cannot parse operand \"{0}\"!", value));

						return new Variant(dValue);
					}

				case VariantType.String:
					return new Variant(value.Trim(new[] { '\"' }));
			}

			return new Variant();
		}

		private static void Rewind()
		{
			if (streamIndex > 0)
				--streamIndex;
		}
	}
}
