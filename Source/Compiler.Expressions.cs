using System;
using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	internal partial class Compiler
	{
		private void Statement()
		{
			while (!stream.IsEndOfStream && !stream.Accept(TokenType.Delimiter, "}"))
				Assignment();
		}

		private void Assignment()
		{
			if (Parser.IsAssignOperation(stream.PeekAhead(1)))
			{
				int variableIndex = GetVariableIndex(stream.GetWord());
				Token operation = stream.Read();

				if (operation.Value != "=")
					Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(variableIndex)));

				Expression();

				switch (operation.Value)
				{
					case "+=":
						Instructions.Add(new Instruction(Opcode.Add));
						break;

					case "-=":
						Instructions.Add(new Instruction(Opcode.Subtract));
						break;

					case "*=":
						Instructions.Add(new Instruction(Opcode.Multiply));
						break;

					case "/=":
						Instructions.Add(new Instruction(Opcode.Divide));
						break;

					case "%=":
						Instructions.Add(new Instruction(Opcode.Modulo));
						break;

					case "|=":
						Instructions.Add(new Instruction(Opcode.BitwiseOr));
						break;

					case "^=":
						Instructions.Add(new Instruction(Opcode.BitwiseXor));
						break;

					case "&=":
						Instructions.Add(new Instruction(Opcode.BitwiseAnd));
						break;

					case "<<=":
						Instructions.Add(new Instruction(Opcode.BitwiseShiftLeft));
						break;

					case ">>=":
						Instructions.Add(new Instruction(Opcode.BitwiseShiftRight));
						break;
				}

				Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));
			}
			else
				Expression();

			stream.Accept(TokenType.Delimiter, ";");
		}

		private void Expression()
		{
			Term();

			while (Parser.IsAddOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "+"))
				{
					Term();
					Instructions.Add(new Instruction(Opcode.Add));
				}
				else if (stream.Accept(TokenType.Delimiter, "-"))
				{
					Term();
					Instructions.Add(new Instruction(Opcode.Subtract));
				}
			}
		}

		private void Term()
		{
			SignedFactor();

			while (Parser.IsMulOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "*"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Multiply));
				}
				else if (stream.Accept(TokenType.Delimiter, "/"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Divide));
				}
				else if (stream.Accept(TokenType.Delimiter, "%"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Modulo));
				}
			}
		}

		private void SignedFactor()
		{
			if (Parser.IsAddOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "+"))
				{
					Factor();

					Instructions.Add(new Instruction(Opcode.AbsoluteValue));
				}
				else if (stream.Accept(TokenType.Delimiter, "-"))
				{
					Factor();

					Instructions.Add(new Instruction(Opcode.LogicalNegate));
				}
			}
			else
				Factor();
		}

		private void Factor()
		{
			if (stream.Accept(TokenType.Word, "print"))
			{
				Expression();
				Instructions.Add(new Instruction(Opcode.Print));
			}
			else if (stream.Accept(TokenType.Word, "len"))
			{
				Expression();
				Instructions.Add(new Instruction(Opcode.GetVariableLength));
			}
			else if (stream.Accept(TokenType.Delimiter, "("))
			{
				Expression();
				stream.Expect(TokenType.Delimiter, ")");
			}
			else if (stream.Accept(TokenType.Delimiter, "["))
			{
				List<Variant> arrayValues = new List<Variant>();

				while (!stream.Accept(TokenType.Delimiter, "]"))
				{
					arrayValues.Add(stream.GetVariant());
					stream.Accept(TokenType.Delimiter, ",");
				}

				Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayValues)));
			}
			else if (stream.Pass(TokenType.Word))
			{
				if (stream.PeekAhead(1).Value == "[")
				{
					Variant arrayVariable = new Variant(GetVariableIndex(stream.GetWord()));
					stream.Expect(TokenType.Delimiter, "[");
					Expression();
					stream.Expect(TokenType.Delimiter, "]");

					Instructions.Add(new Instruction(Opcode.GetArrayVariable, arrayVariable));
				}
				else
					Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(GetVariableIndex(stream.GetWord()))));
			}
			else if (stream.Pass(TokenType.Number) || stream.Pass(TokenType.String))
			{
				Instructions.Add(new Instruction(Opcode.Push, stream.GetVariant()));
			}
			else
			{
				throw new Exception("Expected variable, string or number literal");
			}
		}
	}
}
