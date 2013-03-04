using System.Collections.Generic;
using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		private void Assignment()
		{
			if (IsVariable(Stream.Peek().Value))
			{
				int variableIndex = GetVariableIndex(Stream.GetWord());
				int indexOffset = -1;

				if (Stream.Accept(TokenType.Delimiter, "["))
				{
					indexOffset = Stream.Position;
					while (!Stream.Accept(TokenType.Delimiter, "]"))
						Stream.Read();
				}

				Token operation = Stream.Read();

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
					int forwardPosition = Stream.Position;
					Stream.Position = indexOffset;

					TernaryExpression();

					Instructions.Add(new Instruction(Opcode.SetArrayVariable, new Variant(variableIndex)));

					Stream.Position = forwardPosition;
				}
			}
			else
				TernaryExpression();

			Stream.Accept(TokenType.Delimiter, ";");
		}

		private void TernaryExpression()
		{
			LogicalAndOr();

			while (Stream.Accept(TokenType.Delimiter, "?"))
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

				Stream.Expect(TokenType.Delimiter, ":");

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

			while (Parser.IsLogicalAndOrOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "&&"))
				{
					BitwiseXorOr();

					Instructions.Add(new Instruction(Opcode.CompareEqual));
				}
				else if (Stream.Accept(TokenType.Delimiter, "||"))
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

			while (Parser.IsBitwiseXorOrOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "^"))
				{
					BitwiseAnd();
					Instructions.Add(new Instruction(Opcode.BitwiseXor));
				}
				else if (Stream.Accept(TokenType.Delimiter, "|"))
				{
					BitwiseAnd();
					Instructions.Add(new Instruction(Opcode.BitwiseOr));
				}
			}
		}

		private void BitwiseAnd()
		{
			RelationTerm();

			while (Stream.Accept(TokenType.Delimiter, "&"))
			{
				RelationTerm();
				Instructions.Add(new Instruction(Opcode.BitwiseAnd));
			}
		}

		private void RelationTerm()
		{
			RelationGlTerm();

			while (Parser.IsRelationOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "=="))
				{
					RelationGlTerm();
					Instructions.Add(new Instruction(Opcode.CompareEqual));
				}
				else if (Stream.Accept(TokenType.Delimiter, "!="))
				{
					RelationGlTerm();
					Instructions.Add(new Instruction(Opcode.CompareNotEqual));
				}
			}
		}

		private void RelationGlTerm()
		{
			ShiftTerm();

			while (Parser.IsRelationGlOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "<"))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareLesserThan));
				}
				else if (Stream.Accept(TokenType.Delimiter, ">"))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareGreaterThan));
				}
				else if (Stream.Accept(TokenType.Delimiter, "<="))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareLesserThanOrEqual));
				}
				else if (Stream.Accept(TokenType.Delimiter, ">="))
				{
					ShiftTerm();
					Instructions.Add(new Instruction(Opcode.CompareGreaterThanOrEqual));
				}
			}
		}

		private void ShiftTerm()
		{
			Expression();

			while (Parser.IsBitwiseShiftOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "<<"))
				{
					Expression();
					Instructions.Add(new Instruction(Opcode.BitwiseShiftLeft));
				}
				else if (Stream.Accept(TokenType.Delimiter, ">>"))
				{
					Expression();
					Instructions.Add(new Instruction(Opcode.BitwiseShiftRight));
				}
			}
		}

		private void Expression()
		{
			Term();

			while (Parser.IsAddOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "+"))
				{
					Term();
					Instructions.Add(new Instruction(Opcode.Add));
				}
				else if (Stream.Accept(TokenType.Delimiter, "-"))
				{
					Term();
					Instructions.Add(new Instruction(Opcode.Subtract));
				}
			}
		}

		private void Term()
		{
			SignedFactor();

			while (Parser.IsMulOperation(Stream.Peek()))
			{
				if (Stream.Accept(TokenType.Delimiter, "*"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Multiply));
				}
				else if (Stream.Accept(TokenType.Delimiter, "/"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Divide));
				}
				else if (Stream.Accept(TokenType.Delimiter, "%"))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Modulo));
				}
			}
		}

		private void SignedFactor()
		{
			if (Stream.Accept(TokenType.Delimiter, "+"))
			{
				Factor();

				Instructions.Add(new Instruction(Opcode.AbsoluteValue));
			}
			else if (Stream.Accept(TokenType.Delimiter, "-"))
			{
				Factor();

				Instructions.Add(new Instruction(Opcode.LogicalNegate));
			}
			else if (Stream.Accept(TokenType.Delimiter, "!"))
			{
				Factor();
				Instructions.Add(new Instruction(Opcode.BitwiseNot));
			}
			else if (Stream.Accept(TokenType.Delimiter, "~"))
			{
				Factor();
				Instructions.Add(new Instruction(Opcode.BitwiseNegate));
			}
			else if (Stream.Accept(TokenType.Delimiter, "++"))
			{
				int variableIndex = GetVariableIndex(Stream.GetWord());
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(1) }));
				Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(variableIndex)));
			}
			else if (Stream.Accept(TokenType.Delimiter, "--"))
			{
				int variableIndex = GetVariableIndex(Stream.GetWord());
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(-1) }));
				Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(variableIndex)));
			}
			else if (Parser.IsTypeCastOperation(Stream.Peek()) && Stream.PeekAhead(1).Value == "(")
			{
				TypeCast();
			}
			else
				Factor();
		}

		private void Factor()
		{
			if (Stream.Accept(TokenType.Word, "print"))
			{
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.Print));
			}
			else if (Stream.Accept(TokenType.Word, "len"))
			{
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.GetVariableLength));
			}
			else if (Stream.Accept(TokenType.Word, "nil"))
			{
				Instructions.Add(new Instruction(Opcode.Push, new Variant()));
			}
			else if (Stream.Accept(TokenType.Delimiter, "("))
			{
				TernaryExpression();
				Stream.Expect(TokenType.Delimiter, ")");
			}
			else if (Stream.Accept(TokenType.Delimiter, "["))
			{
				int varIndex = AddTempVariable();
				ArrayDeclaration(varIndex);
				Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(varIndex)));
			}
			else if (Stream.Pass(TokenType.Word))
			{
				switch (Stream.PeekAhead(1).Value)
				{
					case "[":
						{
							Variant arrayVariable = new Variant(GetVariableIndex(Stream.GetWord()));
							Stream.Expect(TokenType.Delimiter, "[");
							Expression();
							Stream.Expect(TokenType.Delimiter, "]");

							Instructions.Add(new Instruction(Opcode.GetArrayVariable, arrayVariable));
							break;
						}

					case "(":
						MethodCall();
						break;

					default:
						Instructions.Add(new Instruction(Opcode.GetVariable, new Variant(GetVariableIndex(Stream.GetWord()))));
						break;
				}
			}
			else if (Stream.Pass(TokenType.Number) || Stream.Pass(TokenType.String))
			{
				Instructions.Add(new Instruction(Opcode.Push, Stream.GetVariant()));
			}
			else if (!Parser.IsIncrementOperation(Stream.Peek()))
				Stream.Expected("builtin function, variable or literal");

			if (Stream.Accept(TokenType.Delimiter, "++"))
			{
				int variableIndex = GetVariableIndex(Stream.PeekAhead(-2).Value);
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(1) }));
			}
			else if (Stream.Accept(TokenType.Delimiter, "--"))
			{
				int variableIndex = GetVariableIndex(Stream.PeekAhead(-2).Value);
				Instructions.Add(new Instruction(Opcode.IncrementVariable, new List<Variant> { new Variant(variableIndex), new Variant(-1) }));
			}
		}

		private void TypeCast()
		{
			Token castType = Stream.Read();

			Stream.Expect(TokenType.Delimiter, "(");
			TernaryExpression();
			Stream.Expect(TokenType.Delimiter, ")");

			switch(castType.Value)
			{
				case "int":
					Instructions.Add(new Instruction(Opcode.CastVariant, new Variant((int)VariantType.Int64)));
					break;

				case "double":
					Instructions.Add(new Instruction(Opcode.CastVariant, new Variant((int)VariantType.Double)));
					break;

				case "string":
					Instructions.Add(new Instruction(Opcode.CastVariant, new Variant((int)VariantType.String)));
					break;
			}
		}

		// TODO This should probably be moved out of here
		private void ArrayDeclaration(int variableIndex)
		{
			//stream.Expect(TokenType.Delimiter, "[");

			if (Stream.PeekAhead(1).Value == "..")
			{
				List<Variant> arrayInitializer = new List<Variant>();

				Variant min = Stream.GetVariant();
				Stream.Expect(TokenType.Delimiter, "..");
				Variant max = Stream.GetVariant();

				if (min.Type != VariantType.Int64 || max.Type != VariantType.Int64)
					Stream.Error("Low and high range initializers must be Int64s");

				Stream.Expect(TokenType.Delimiter, "]");

				for (int i = 0; i < max.IntValue - min.IntValue + 1; ++i)
					arrayInitializer.Add(new Variant());

				Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayInitializer)));
				Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));
			}
			else
			{
				List<Variant> arrayInitializer = new List<Variant>();
				Stream.PushPosition();

				while (!Stream.Accept(TokenType.Delimiter, "]"))
				{
					arrayInitializer.Add(new Variant());

					while (!Stream.Accept(TokenType.Delimiter, ","))
					{
						if (Stream.Pass(TokenType.Delimiter, "]"))
							break;

						++Stream.Position;
					}
				}

				Instructions.Add(new Instruction(Opcode.Push, new Variant(arrayInitializer)));
				Instructions.Add(new Instruction(Opcode.SetVariable, new Variant(variableIndex)));

				Stream.PopPosition();

				for (int i = 0; i < arrayInitializer.Count; ++i)
				{
					TernaryExpression();
					Instructions.Add(new Instruction(Opcode.Push, new Variant(i)));
					Instructions.Add(new Instruction(Opcode.SetArrayVariable, new Variant(variableIndex)));

					Stream.Accept(TokenType.Delimiter, ",");
				}

				Stream.Expect(TokenType.Delimiter, "]");
			}
		}

		private void MethodCall()
		{
			string functionName = Stream.GetWord();
			int argCount = 0;

			// Verify the proper number of arguments were passed
			Stream.Expect(TokenType.Delimiter, "(");
			while (!Stream.Accept(TokenType.Delimiter, ")"))
			{
				TernaryExpression();
				Stream.Accept(TokenType.Delimiter, ",");
				++argCount;
			}

			if (CurrentClass == null)
			{
				if (CurrentModule.GetMethod(functionName).ArgumentCount != argCount)
					Stream.Error("Expected {0} arguments to be passed to \"{1}\", {2} were passed.",
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
					Stream.Error("Expected {0} arguments to be passed to \"{1}\", {2} were passed.",
						CurrentClass.GetMethod(functionName).ArgumentCount, functionName, argCount);

				Instructions.Add(new Instruction(Opcode.GetThis));
				Instructions.Add(new Instruction(Opcode.ClassCall, new Variant(CurrentClass.GetMethodIndex(functionName))));
			}
		}
	}
}
