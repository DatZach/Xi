﻿using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	internal partial class Compiler
	{
		private void Assignment()
		{
			if (IsVariable(stream.Peek().Value))
			{
				int variableIndex = GetVariableIndex(stream.GetWord());
				int indexOffset = -1;

				if (stream.Accept(TokenType.Delimiter, "["))
				{
					indexOffset = stream.Position;
					while (!stream.Accept(TokenType.Delimiter, "]"))
						stream.Read();
				}

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

				if (indexOffset == -1)
					Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));
				else
				{
					int forwardPosition = stream.Position;
					stream.Position = indexOffset;

					TernaryExpression();

					Instructions.Add(new Instruction(Opcode.SetArrayVariable, new Variant(variableIndex)));

					stream.Position = forwardPosition;
				}
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

				labelElse.Fix();
				labelEnd.Fix();
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

			while (Parser.IsBitwiseXorOrOperation(stream.Peek()))
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

			while (stream.Accept(TokenType.Delimiter, "&"))
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
			else if (stream.Accept(TokenType.Delimiter, "++"))
			{
				int variableIndex = GetVariableIndex(stream.GetWord());
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(1) }));
				Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(variableIndex)));
			}
			else if (stream.Accept(TokenType.Delimiter, "--"))
			{
				int variableIndex = GetVariableIndex(stream.GetWord());
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(-1) }));
				Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(variableIndex)));
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
			else if (stream.Accept(TokenType.Word, "nil"))
			{
				Instructions.Add(new Instruction(Opcode.Push, new Variant()));
			}
			else if (stream.Accept(TokenType.Delimiter, "("))
			{
				TernaryExpression();
				stream.Expect(TokenType.Delimiter, ")");
			}
			else if (stream.Accept(TokenType.Delimiter, "["))
			{
				ArrayDeclaration(AddTempVariable());
			}
			else if (stream.Pass(TokenType.Word))
			{
				switch (stream.PeekAhead(1).Value)
				{
					case "[":
						{
							Variant arrayVariable = new Variant(GetVariableIndex(stream.GetWord()));
							stream.Expect(TokenType.Delimiter, "[");
							Expression();
							stream.Expect(TokenType.Delimiter, "]");

							Instructions.Add(new Instruction(Opcode.GetArrayVariable, arrayVariable));
							break;
						}

					case "(":
						MethodCall();
						break;

					default:
						Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(GetVariableIndex(stream.GetWord()))));
						break;
				}
			}
			else if (stream.Pass(TokenType.Number) || stream.Pass(TokenType.String))
			{
				Instructions.Add(new Instruction(Opcode.Push, stream.GetVariant()));
			}
			else if (!Parser.IsIncrementOperation(stream.Peek()))
				stream.Expected("builtin function, variable or literal");

			if (stream.Accept(TokenType.Delimiter, "++"))
			{
				int variableIndex = GetVariableIndex(stream.PeekAhead(-2).Value);
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(1) }));
			}
			else if (stream.Accept(TokenType.Delimiter, "--"))
			{
				int variableIndex = GetVariableIndex(stream.PeekAhead(-2).Value);
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(-1) }));
			}
		}

		// TODO This should probably be moved out of here
		private void ArrayDeclaration(int variableIndex)
		{
			//stream.Expect(TokenType.Delimiter, "[");

			if (stream.PeekAhead(1).Value == "..")
			{
				List<Variant> arrayInitializer = new List<Variant>();

				Variant min = stream.GetVariant();
				stream.Expect(TokenType.Delimiter, "..");
				Variant max = stream.GetVariant();

				if (min.Type != VariantType.Int64 || max.Type != VariantType.Int64)
					stream.Error("Low and high range initializers must be Int64s");

				stream.Expect(TokenType.Delimiter, "]");

				for (int i = 0; i < max.IntValue - min.IntValue + 1; ++i)
					arrayInitializer.Add(new Variant());

				Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayInitializer)));
				Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));
			}
			else
			{
				List<Variant> arrayInitializer = new List<Variant>();
				stream.PushPosition();

				while (!stream.Accept(TokenType.Delimiter, "]"))
				{
					arrayInitializer.Add(new Variant());

					while (!stream.Accept(TokenType.Delimiter, ","))
					{
						if (stream.Pass(TokenType.Delimiter, "]"))
							break;

						++stream.Position;
					}
				}

				Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayInitializer)));
				Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));

				stream.PopPosition();

				for (int i = 0; i < arrayInitializer.Count; ++i)
				{
					TernaryExpression();
					Instructions.Add(new Instruction(Opcode.Push, new Variant(i)));
					Instructions.Add(new Instruction(Opcode.SetArrayVariable, new Variant(variableIndex)));

					stream.Accept(TokenType.Delimiter, ",");
				}

				stream.Expect(TokenType.Delimiter, "]");
			}
		}

		private void MethodCall()
		{
			string functionName = stream.GetWord();
			int argCount = 0;

			// Verify the proper number of arguments were passed
			stream.Expect(TokenType.Delimiter, "(");
			while (!stream.Accept(TokenType.Delimiter, ")"))
			{
				TernaryExpression();
				stream.Accept(TokenType.Delimiter, ",");
				++argCount;
			}

			if (CurrentClass == null)
			{
				if (CurrentModule.GetMethod(functionName).ArgumentCount != argCount)
					stream.Error("Expected {0} arguments to be passed to \"{1}\", {2} were passed.",
						CurrentModule.GetMethod(functionName).ArgumentCount, functionName, argCount);

				List<Variant> operands = new List<Variant>
						{
							new Variant(Modules.Count - 1),
							new Variant(CurrentModule.GetMethodIndex(functionName))
						};

				Instructions.Add(new Instruction(Opcode.ModuleCall, operands));
			}
			else
			{
				if (CurrentClass.GetMethod(functionName).ArgumentCount != argCount)
					stream.Error("Expected {0} arguments to be passed to \"{1}\", {2} were passed.",
						CurrentClass.GetMethod(functionName).ArgumentCount, functionName, argCount);

				Instructions.Add(new Instruction(Opcode.GetThis));
				Instructions.Add(new Instruction(Opcode.ClassCall, new Variant(CurrentClass.GetMethodIndex(functionName))));
			}
		}
	}
}
