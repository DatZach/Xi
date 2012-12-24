/*
 *	example.xi
 *	Example of the Xi scripting language
 */

/* Imports modules from search directories */
import Os, Crytpo;

/* Name the module this file contains, 1 module per file */
module Example;

// Private to module
int animalsCreated = 0;

public func GetAnimalsCreated() : int
{
	return animalsCreated;
}

abstract class Animal
{
	// Public variable A
	public int a = 1234;
	
	// Private variable voice
	string voice;
	
	this(string voice)
	{
		this.voice = voice;
		++animalsCreated;
	}
	
	protected func Speak() : void
	{
		print voice;
	}
}

public class Cat : Animal
{
	this() : base("Meow");
}

public class Dog : Animal
{
	this() : base("Woof");
}
