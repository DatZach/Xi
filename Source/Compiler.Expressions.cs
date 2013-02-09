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

				TernaryExpression();

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
				TernaryExpression();

			stream.Accept(TokenType.Delimiter, ";");
		}

		private void TernaryExpression()
		{
			LogicalAndOr();

			while (stream.Accept(TokenType.Delimiter, "?"))
			{
				Label labelElse = new Label(this);
				Label labelEnd = new Label(this);

				Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
				Instructions.Add(new Instruction(Opcode.CompareEqual));
				Instructions.Add(new Instruction(Opcode.IfFalse, new Variant(0)));
				labelElse.PatchHere();

				LogicalAndOr();
				Instructions.Add(new Instruction(Opcode.Jump, new Variant(0)));
				labelEnd.PatchHere();

				stream.Expect(TokenType.Delimiter, ":");

				labelElse.Mark();
				LogicalAndOr();
				labelEnd.Mark();
			}
		}

		private void LogicalAndOr()
		{
			BitwiseXorOr();

			while (Parser.IsLogicalAndOrOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "&&"))
				{
					BitwiseXorOr();

					Instructions.Add(new Instruction(Opcode.CompareEqual));
				}
				else if (stream.Accept(TokenType.Delimiter, "||"))
				{
					BitwiseXorOr();

					Instructions.Add(new Instruction(Opcode.Add));
					Instructions.Add(new Instruction(Opcode.Push, new Variant(0)));
					Instructions.Add(new Instruction(Opcode.CompareGreaterThan));
				}
			}
		}

		private void BitwiseXorOr()
		{
			BitwiseAnd();

			while(Parser.IsBitwiseXorOrOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "^"))
				{
					BitwiseAnd();
					Instructions.Add(new Instruction(Opcode.BitwiseXor));
				}
				else if (stream.Accept(TokenType.Delimiter, "|"))
				{
					BitwiseAnd();
					Instructions.Add(new Instruction(Opcode.BitwiseOr));
				}
			}
		}

		private void BitwiseAnd()
		{
			RelationTerm();

			while(stream.Accept(TokenType.Delimiter, "&"))
			{
				RelationTerm();
				Instructions.Add(new Instruction(Opcode.BitwiseAnd));
			}
		}

		private void RelationTerm()
		{
			RelationGlTerm();

			while (Parser.IsRelationOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "=="))
				{
					RelationGlTerm();
					Instructions.Add(new Instruction(Opcode.CompareEqual));
				}
				else if (stream.Accept(TokenType.Delimiter, "!="))
				{
					RelationGlTerm();
					Instructions.Add(new Instruction(Opcode.CompareNotEqual));
				}
			}
		}

		private void RelationGlTerm()
		{
			ShiftTerm();

			while (Parser.IsRelationGlOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "<"))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareLesserThan));
				}
				else if (stream.Accept(TokenType.Delimiter, ">"))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareGreaterThan));
				}
				else if (stream.Accept(TokenType.Delimiter, "<="))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareLesserThanOrEqual));
				}
				else if (stream.Accept(TokenType.Delimiter, ">="))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareGreaterThanOrEqual));
				}
			}
		}

		private void ShiftTerm()
		{
			Expression();

			while (Parser.IsBitwiseShiftOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Delimiter, "<<"))
				{
					Expression();
					Instructions.Add(new Instruction(Opcode.BitwiseShiftLeft));
				}
				else if (stream.Accept(TokenType.Delimiter, ">>"))
				{
					Expression();
					Instructions.Add(new Instruction(Opcode.BitwiseShiftRight));
				}
			}
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
			else if (stream.Accept(TokenType.Delimiter, "!"))
			{
				Factor();
				Instructions.Add(new Instruction(Opcode.BitwiseNot));
			}
			else if (stream.Accept(TokenType.Delimiter, "~"))
			{
				Factor();
				Instructions.Add(new Instruction(Opcode.BitwiseNegate));
			}
			else
				Factor();
		}

		private void Factor()
		{
			if (stream.Accept(TokenType.Word, "print"))
			{
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.Print));
			}
			else if (stream.Accept(TokenType.Word, "len"))
			{
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.GetVariableLength));
			}
			else if (stream.Accept(TokenType.Delimiter, "("))
			{
				TernaryExpression();
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
				Expected("builtin function, variable or literal");
		}
	}
}
