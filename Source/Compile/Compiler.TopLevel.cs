using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		private void Module(string name)
		{
			AddModule(name);

			while (!Stream.EndOfStream)
			{
				if (Stream.Pass("using"))
					UsingStatement();
				else if (Stream.Pass("class"))
					ClassDeclaration();
				else if (Stream.Pass("function"))
					FunctionDeclaration();
				else if (Stream.Pass(TokenType.Delimiter, "{"))
					OrphanDeclaration();
				else
				{
					if (CurrentModule.Body == null)
						AddModuleBody();

					BlockStatement();
				}
			}

			Instructions.Add(new Instruction(Opcode.Return, new Variant(0)));

			if (CurrentModule.Body != null)
				LeaveMethod();

			LeaveModule();
		}

		private void UsingStatement()
		{
			Stream.Expect(TokenType.Word, "using");

			string modulePath = "";

			do
			{
				modulePath += Stream.Read().Value;
			} while (!Stream.Accept(TokenType.Delimiter, ";"));

			string path = Path.Combine(modulePath.Split(new[ ] { '.' }));
			path += ".xi";

			List<Token> tokenStream = Tokenizer.ParseFile(path);
			if (tokenStream == null)
				return;

			if (!Compile(new TokenStream(tokenStream)))
				Stream.Error("Could not compile module \"{0}\"", modulePath);

			if (CurrentModule.Body == null)
				AddModuleBody();

			Instructions.Add(new Instruction(Opcode.ModuleCall, new List<Variant> { new Variant(Modules.Count - 1), new Variant(-1) }));
		}

		private void ClassDeclaration()
		{
			// TODO allow for multiple inherited classes

			Stream.Expect(TokenType.Word, "class");

			// Get class name & base name
			string name = Stream.GetWord();
			string baseName = "";

			// Do we have a base class?
			if (Stream.Accept(TokenType.Delimiter, ":"))
				baseName = Stream.GetWord();

			// Add class
			AddClass(name, CurrentModule.Classes.Find(c => c.Name == baseName));

			Stream.Expect(TokenType.Delimiter, "{");

			while (!Stream.Accept(TokenType.Delimiter, "}"))
			{
				ClassField();
				// <constructor>
				// <destructor>
				// <class-function>
			}
		}

		private void OrphanDeclaration()
		{
			if (CurrentModule.Name == Modules.First().Name)
			{
				if (CurrentModule.Body == null)
					AddModuleBody();

				Block();
			}
			else
			{
				Stream.Expect(TokenType.Delimiter, "{");

				while (!Stream.Accept(TokenType.Delimiter, "}"))
					Stream.Read();
			}
		}

		private void ClassField()
		{
			
		}

		private void VariableDeclaration()
		{
			bool globalVariable = Stream.Accept(TokenType.Word, "global");

			if (Stream.Accept(TokenType.Word, "var"))
			{
				do
				{
					string name = Stream.GetWord();

					AddVariable(name);

					if (Stream.Accept(TokenType.Delimiter, "="))
					{
						if (Stream.Accept(TokenType.Delimiter, "["))
						{
							ArrayDeclaration(GetVariableIndex(name));
						}
						else
						{
							TernaryExpression();
							Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(GetVariableIndex(name))));
						}
					}
					else
					{
						Instructions.Add(new Instruction(Opcode.Push, new Variant()));
						Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(GetVariableIndex(name))));
					}
				} while(Stream.Accept(TokenType.Delimiter, ","));
			}
			else if (globalVariable)
				Stream.Expected("keyword \"var\" after keyword \"global\".");

			Stream.Accept(TokenType.Delimiter, ";");
		}

		private void FunctionDeclaration()
		{
			Stream.Expect(TokenType.Word, "function");

			string functionName = Stream.GetWord();
			List<string> arguments = new List<string>();

			if (Stream.Accept(TokenType.Delimiter, ":"))
			{
				do
				{
					arguments.Add(Stream.GetWord());
				} while (Stream.Accept(TokenType.Delimiter, ","));
			}

			AddMethod(functionName, arguments.Count);
			foreach(string arg in arguments)
				AddVariable(arg);

			Block();

			// Add protective return
			if (Instructions.Last().Opcode != Opcode.Return)
				Instructions.Add(new Instruction(Opcode.Return, new Variant(0)));

			LeaveMethod();
		}

		private void Block()
		{
			Stream.Expect(TokenType.Delimiter, "{");

			do
			{
				BlockStatement();
			} while (!Stream.EndOfStream && !Stream.Accept(TokenType.Delimiter, "}"));
		}

		private void BlockStatement()
		{
			if (Stream.Accept(TokenType.Delimiter, "{"))
			{
				do
				{
					BlockStatementInner();
				} while (!Stream.Accept(TokenType.Delimiter, "}"));
			}
			else
				BlockStatementInner();
		}

		private void BlockStatementInner()
		{
			switch (Stream.Peek().Value)
			{
				case "if":
					IfStatement();
					break;

				case "while":
					WhileStatement();
					break;

				case "return":
					ReturnStatement();
					break;

				case "var":
					VariableDeclaration();
					break;

				default:
					Assignment();
					break;
			}
		}
	}
}
