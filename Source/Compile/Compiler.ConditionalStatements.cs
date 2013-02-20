using Xi.Lexer;
using Xi.Vm;

namespace Xi.Compile
{
	internal partial class Compiler
	{
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
