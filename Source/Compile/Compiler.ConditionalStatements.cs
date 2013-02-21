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

			Assignment();

			Instructions.Add(new Instruction(Opcode.Jump, new Variant(0)));
			labelEnd.PatchHere();

			labelElse.Mark();
			if (stream.Accept(TokenType.Word, "else"))
				Assignment();

			labelEnd.Mark();
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
