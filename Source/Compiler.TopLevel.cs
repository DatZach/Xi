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
					OrphanDeclaration(); // TODO This is a dirty hack
			}
		}

		private void UsingStatement()
		{
			
		}

		private void ClassDeclaration()
		{
			// TODO allow for multiple inherited classes
			
			// Get class name & base name
			string name = stream.GetWord();
			string baseName = "";

			// Do we have a base class?
			if (stream.Accept(TokenType.Delimiter, ":"))
				baseName = stream.GetWord();

			// Add class
			AddClass(name, Classes.Find(c => c.Name == baseName));

			stream.Expect(TokenType.Delimiter, "{");

			while(!stream.Accept(TokenType.Delimiter, "}"))
			{
				// <class-variable>
				// <constructor>
				// <destructor>
				// <class-function>
			}
		}

		private void OrphanDeclaration()
		{
			AddClass(ClassNameDefault);
			AddMethod(MethodNameEntry);

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
