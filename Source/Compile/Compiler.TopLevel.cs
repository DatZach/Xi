using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	partial class Compiler
	{
		private void Module(string name)
		{
			AddModule(name);

			while (!stream.EndOfStream)
			{
				if (stream.Pass("using"))
					UsingStatement();
				else if (stream.Pass("class"))
					ClassDeclaration();
				else if (stream.Pass("function"))
					FunctionDeclaration();
				else if (stream.Pass(TokenType.Delimiter, "{"))
					OrphanDeclaration();
				else
					BodyDeclaration();
			}
		}

		private void UsingStatement()
		{
			stream.Expect(TokenType.Word, "using");
		}

		private void ClassDeclaration()
		{
			// TODO allow for multiple inherited classes

			stream.Expect(TokenType.Word, "class");

			// Get class name & base name
			string name = stream.GetWord();
			string baseName = "";

			// Do we have a base class?
			if (stream.Accept(TokenType.Delimiter, ":"))
				baseName = stream.GetWord();

			// Add class
			AddClass(name, CurrentModule.Classes.Find(c => c.Name == baseName));

			stream.Expect(TokenType.Delimiter, "{");

			while (!stream.Accept(TokenType.Delimiter, "}"))
			{
				// <class-variable>
				// <constructor>
				// <destructor>
				// <class-function>
			}
		}

		private void OrphanDeclaration()
		{
			AddModuleBody();
			Block();
		}

		private void BodyDeclaration()
		{
			AddModuleBody();

			do
			{
				if (stream.Accept(TokenType.Word, "return"))
				{
					ReturnStatement();
				}
				else
				{
					VariableDeclaration();
					if (stream.EndOfStream)
						break;

					Assignment();
				}
			} while (!stream.EndOfStream);
		}

		private void VariableDeclaration()
		{
			bool globalVariable = stream.Accept(TokenType.Word, "global");

			if (stream.Accept(TokenType.Word, "var"))
			{
				do
				{
					string name = stream.GetWord();

					AddVariable(name);

					if (stream.Accept(TokenType.Delimiter, "="))
					{
						if (stream.Accept(TokenType.Delimiter, "["))
						{
							List<Variant> arrayValues = new List<Variant>();

							while (!stream.Accept(TokenType.Delimiter, "]"))
							{
								arrayValues.Add(stream.GetVariant());
								stream.Accept(TokenType.Delimiter, ",");
							}

							Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayValues)));
						}
						else
							Instructions.Add(new Instruction(Opcode.Push, stream.GetVariant()));

						Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(GetVariableIndex(name))));
					}
					else
					{
						Instructions.Add(new Instruction(Opcode.Push, new Variant()));
						Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(GetVariableIndex(name))));
					}
				} while(stream.Accept(TokenType.Delimiter, ","));
			}
			else if (globalVariable)
				stream.Expected("keyword \"var\" after keyword \"global\".");

			stream.Accept(TokenType.Delimiter, ";");
		}

		private void FunctionDeclaration()
		{
			stream.Expect(TokenType.Word, "function");

			string functionName = stream.GetWord();
			List<string> arguments = new List<string>();

			if (stream.Accept(TokenType.Delimiter, ":"))
			{
				do
				{
					arguments.Add(stream.GetWord());
				} while (stream.Accept(TokenType.Delimiter, ","));
			}

			AddMethod(functionName, arguments.Count);
			foreach(string arg in arguments)
				AddVariable(arg);

			Block();
		}

		private void Block()
		{
			stream.Expect(TokenType.Delimiter, "{");

			do
			{
				if (stream.Accept(TokenType.Word, "return"))
				{
					ReturnStatement();
				}
				else
				{
					VariableDeclaration();
					if (stream.EndOfStream)
						break;

					Assignment();
				}
			} while (!stream.EndOfStream && !stream.Accept(TokenType.Delimiter, "}"));
		}
	}
}
