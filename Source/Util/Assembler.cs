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
					Console.WriteLine("[Error   ] Failed to assemble file!");
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
			instructions.Add(Instruction.OpcodeHasOperands(opcode)
								 ? new Instruction(opcode, ReadOperand())
								 : new Instruction(opcode));
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
			stream = text.Split(' ');

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
			Variant.VariantType type = Variant.VariantType.Int64;
			string value = ReadWord();

			if (value.First() == '\"' && value.Last() == '\"')
				type = Variant.VariantType.String;
			else if (value.IndexOf('.') != 0)
				type = Variant.VariantType.Double;

			switch (type)
			{
				case Variant.VariantType.Int64:
					{
						long iValue;
						if (!Int64.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out iValue))
							throw new Exception(String.Format("Cannot parse operand \"{0}\"!", value));

						return new Variant(iValue);
					}

				case Variant.VariantType.Double:
					{
						double dValue;
						if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out dValue))
							throw new Exception(String.Format("Cannot parse operand \"{0}\"!", value));

						return new Variant(dValue);
					}

				case Variant.VariantType.String:
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
