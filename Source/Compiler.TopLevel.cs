using Xi.Lexer;

namespace Xi
{
	partial class Compiler
	{
		private void Program()
		{
			while (!stream.IsEndOfStream)
			{
				if (stream.Accept(TokenType.Word, "using"))
					UsingStatement();
				else if (stream.Accept(TokenType.Word, "class"))
					ClassDeclaration();
				else if (stream.Accept(TokenType.Delimiter, "{"))
					OrphanDeclaration();
				else if (stream.Accept(TokenType.Word, "var") || stream.Accept(TokenType.Word, "global"))
					VariableDeclaration();
				else
					Block();
			}
		}

		private void UsingStatement()
		{
			
		}

		private void ClassDeclaration()
		{
			
		}

		private void OrphanDeclaration()
		{
			Block();
		}

		private void VariableDeclaration()
		{
			
		}

		private void Block()
		{
			Statement();
		}
	}
}
