using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	internal partial class Compiler
	{
		private void IfStatement()
		{
			Label labelElse = new Label(this);
			Label labelEnd = new Label(this);

			stream.Expect(TokenType.Word, "if");
			stream.Expect(TokenType.Delimiter, "(");

			TernaryExpression();

			stream.Expect(TokenType.Delimiter, ")");

			Instructions.Add(new Instruction(Opcode.Push, new Variant(1)));
			Instructions.Add(new Instruction(Opcode.CompareEqual));
			Instructions.Add(new Instruction(Opcode.IfFalse, new Variant(0)));
			labelElse.PatchHere();

			BlockStatement();

			Instructions.Add(new Instruction(Opcode.Jump, new Variant(0)));
			labelEnd.PatchHere();

			labelElse.Mark();
			if (stream.Accept(TokenType.Word, "else"))
				BlockStatement();

			labelEnd.Mark();

			labelElse.Fix();
			labelEnd.Fix();
		}

		private void WhileStatement()
		{
			Label labelStart = new Label(this);
			Label labelEnd = new Label(this);

			stream.Expect(TokenType.Word, "while");
			stream.Expect(TokenType.Delimiter, "(");

			labelStart.Mark();
			TernaryExpression();
			stream.Expect(TokenType.Delimiter, ")");

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
			stream.Expect(TokenType.Word, "for");
			stream.Expect(TokenType.Delimiter, "(");

			BlockStatement();

			stream.Expect(TokenType.Delimiter, ";");


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
			stream.Expect(TokenType.Word, "return");

			if (!stream.Accept(TokenType.Delimiter, ";"))
			{
				// TODO Check for commas, denoting multiple returns
				TernaryExpression();
				Instructions.Add(new Instruction(Opcode.Return, new Variant(1)));
				stream.Expect(TokenType.Delimiter, ";");
			}
			else
				Instructions.Add(new Instruction(Opcode.Return, new Variant(0)));
		}
	}
}
