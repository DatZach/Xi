using System;
using Xi.Lexer;
using Xi.Vm;

namespace Xi
{
	internal partial class Compiler
	{
		private void PrintExpression()
		{
			if (stream.AcceptWord("print"))
			{
				Expression();
				Instructions.Add(new Instruction(Opcode.Print));
			}
			else
				Expression();
		}

		private void Expression()
		{
			Term();

			while (Parser.IsAddOperation(stream.Peek()))
			{
				if (stream.Accept(TokenType.Add))
				{
					Term();
					Instructions.Add(new Instruction(Opcode.Add));
				}
				else if (stream.Accept(TokenType.Subtract))
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
				if (stream.Accept(TokenType.Multiply))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Multiply));
				}
				else if (stream.Accept(TokenType.Divide))
				{
					SignedFactor();
					Instructions.Add(new Instruction(Opcode.Divide));
				}
				else if (stream.Accept(TokenType.Modulo))
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
				if (stream.Accept(TokenType.Add))
				{
					// Return absolute value of literal/variable
				}
				else if (stream.Accept(TokenType.Subtract))
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
			if (stream.Accept(TokenType.OpenParentheses))
			{
				Expression();
				stream.Expect(TokenType.CloseParentheses);
			}
			else if (stream.Accept(TokenType.Word))
			{
				// Variable
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
