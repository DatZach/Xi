using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	public partial class Compiler
	{
		private void IfStatement()
		{
			Label labelElse = new Label(this);
			Label labelEnd = new Label(this);

			Stream.Expect(TokenType.Word, "if");
			Stream.Expect(TokenType.Delimiter, "(");

			TernaryExpression();

			Stream.Expect(TokenType.Delimiter, ")");

			Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
			Instructions.Add(new Instruction(Opcode.CompareEqual));
			Instructions.Add(new Instruction(Opcode.IfFalse, new Variant(0)));
			labelElse.PatchHere();

			BlockStatement();

			Instructions.Add(new Instruction(Opcode.Jump, new Variant(0)));
			labelEnd.PatchHere();

			labelElse.Mark();
			if (Stream.Accept(TokenType.Word, "else"))
				BlockStatement();

			labelEnd.Mark();

			labelElse.Fix();
			labelEnd.Fix();
		}

		private void WhileStatement()
		{
			Label labelStart = new Label(this);
			Label labelEnd = new Label(this);

			Stream.Expect(TokenType.Word, "while");
			Stream.Expect(TokenType.Delimiter, "(");

			labelStart.Mark();
			TernaryExpression();
			Stream.Expect(TokenType.Delimiter, ")");

			Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
			Instructions.Add(new Instruction(Opcode.CompareEqual));
			Instructions.Add(new Instruction(Opcode.IfFalse, new Variant(0)));
			labelEnd.PatchHere();

			BlockStatement();

			Instructions.Add(new Instruction(Opcode.Jump, new Variant(0)));
			labelStart.PatchHere();

			labelEnd.Mark();

			labelStart.Fix();
			labelEnd.Fix();
		}

		private void ForStatement()
		{
			Stream.Expect(TokenType.Word, "for");
			Stream.Expect(TokenType.Delimiter, "(");

			// TODO Fix this
			TernaryExpression();

			Stream.Expect(TokenType.Delimiter, ";");

			Label condition = new Label(this);
			TernaryExpression();
			Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
			Instructions.Add(new Instruction(Opcode.CompareEqual));
			Instructions.Add(new Instruction(Opcode.IfFalse, new Variant(0)));
			condition.PatchHere();
		}

		private void ForeachStatement()
		{
			
		}

		private void SwitchStatement()
		{
			
		}

		private void ContinueStatement()
		{
			
		}

		private void BreakStatement()
		{
			
		}

		private void ReturnStatement()
		{
			Stream.Expect(TokenType.Word, "return");

			if (!Stream.Accept(TokenType.Delimiter, ";"))
			{
				// TODO Check for commas, denoting multiple returns
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.Return, new Variant(1)));
				Stream.Expect(TokenType.Delimiter, ";");
			}
			else
				Instructions.Add(new Instruction(Opcode.Return, new Variant(0)));
		}
	}
}
