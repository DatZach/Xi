using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xi.Lexer;

namespace Xi
{
	partial class Compiler
	{
		private void Program()
		{
			while (!stream.IsEndOfStream)
			{
				if (stream.AcceptWord("using"))
					UsingStatement();
				else if (stream.AcceptWord("class"))
					ClassDeclaration();
				else if (stream.Accept(TokenType.OpenCurlyBracket))
					OrphanDeclaration();
				else if (stream.Accept(TokenType.KeywordVariant) || stream.AcceptWord("global"))
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
