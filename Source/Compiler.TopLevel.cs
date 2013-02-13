﻿using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	partial class Compiler
	{
		private void Module(string name)
		{
			AddModule(name);

			while (!stream.IsEndOfStream)
			{
				if (stream.Accept(TokenType.Word, "using"))
					UsingStatement();
				else if (stream.Accept(TokenType.Word, "class"))
					ClassDeclaration();
				else if (stream.Accept(TokenType.Delimiter, "{"))
					OrphanDeclaration();
				else
					OrphanDeclaration(); // TODO This is a dirty hack
			}
		}

		private void UsingStatement()
		{

		}

		private void ClassDeclaration()
		{
			// TODO allow for multiple inherited classes

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
			//AddClass(ClassNameDefault);
			//AddMethod(MethodNameEntry);
			AddModuleBody();

			Block();
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
				Expected("keyword \"var\" after keyword \"global\".");

			stream.Accept(TokenType.Delimiter, ";");
		}

		private void FunctionDeclaration()
		{
			
		}

		private void Block()
		{
			VariableDeclaration();
			Statement();
		}
	}
}
