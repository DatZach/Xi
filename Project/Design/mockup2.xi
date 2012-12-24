/*
 *	mockup2.xi
 *	Mockup of possible Xi syntax
 *	Revision 2.8
 *	December 20th, 2012
 */

/*
 *	Naming conventions
 *
 *	Classes, methods, functions, and public members should be CamelCased.
 *	Variables and private members should be camelCased.
 */

/*
 *	Variant
 *	
 *	All variables and classes are variants, variants are objects
 *		allocated and managed by the VM and the VM's GC. Since they're
 *		objects they're passed by reference to objects unless a copy is
 *		explicitly made via the "copy" operator.
 *
 *	Variant Types
 *		- int64
 *		- Double
 *		- String (UTF-8)
 *		- Object
 *		- Array
 *		- Null
 *
 *	Real Values
 *		All number variables are represented as a double, refered to as a "real".
 *		All operations can be made on numbers, regardless of the primitive type,
 *			this includes binary operations, | ! ~ ^ &
 *
 *	String Values
 *		All string variables are represented as a UTF-8 string.
 *
 *	Object Values
 *		A const void* pointer to some object.
 *
 *	Null Values
 *		Any variable that hasn't been assigned a value yet will be initialized as null type
 *		These can adopt any type, after a type has been adopted it must be typecasted in order
 *		to be used
 *
 *	Variant Methods
 *		Since variants are objects they have methods which can be called like every other class
 *			these are handled by the VM's internal implementation rather than a user defined
 *			method, which makes these a little bit special holding special functions with the VM.
 *
 *		Soft Casting
 *			This will copy a new object for use but will not cast the object itself
 *
 *			.str
 *			.int
 *			.real
 *			.array
 *			.object
 *
 *		Hard Casting
 *			This will typecast the variant into a new type, attempting to carry over
 *				whatever data is present into the new type
 *
 *			.tostr
 *			.toreal
 *			.toint
 *
 *		String Methods
 *			.length
 *			[ To expand on ]
 *
 *		Real Methods
 *			[ To expand on ]
 *
 *		Object Methods
 *			[ To expand on ]
 */

/*
 *	using will import a module's namespace into global,
 *	based around C++ module header replacement idea
 *
 *	Appending an * will import a module and its namespace
 *	will not be brought into global namespace
 *	
 *	Imported
 *		Random
 *		PrintLine
 *		GetLine
 *		
 */
 
using Std;
using XyzLibrary *;

/*
 *	These variables are public to C# 4.0
 */

global var someGlobalVariable = 0;

/*
 *	Define a simple class
 */

class Animal
{
	// Yes, I stole accessors from C#
	protected var Name { get; private set; }
	
	/*
	 *	Constructor -- Dart "style"
	 *		The parameter list contains a variable with the same name as member
	 *		it's assumed that this is meaning to be initialized
	 */
	 
	public constructor : Name
	{
		PrintLine("Animal has been created!");
	}
	
	/*
	 *	Stealing anothert Dart feature,	ff a function has no body line in the case
	 *	of virtual functions or constructors, you can simply finish the line with
	 *	a semicolon
	 *
	 *	public constructor : Name;
	 *
	 */
	
	public deconstructor
	{
		PrintLine("Animal destroyed!");
	}
	
	// Public function
	public function Speak
	{
		PrintLine("I'm an animal!");
	}
	
	// Protected function
	protected function Eat
	{
		PrintLine("Eating something...");
	}
	
	protected function DoSomethingWithLife;
	
	// Private function
	private function IdkMan
	{
	
	}
	
	// Static methods and fields
	public static var Foo;
	
	public static function Asdf
	{
	
	}
}

class Cat : Animal
{
	public constructor
	{
		base("Cat");
		PrintLine("Cat has been created!");
	}
	
	// Override function inherited from parent
	public function Speak
	{
		PrintLine("Meow!");
	}
}

class Dog : Animal
{
	public constructor
	{
		base("Dog");
		PrintLine("Dog has been created!");
	}
	
	// Override function inherited from parent
	public function Speak
	{
		PrintLine("Woof!");
		base.Speak();
	}
}

function PickName
{
	var names = [ "Mary", "Jane", "Sue", "Carl", "Derek", "Stan" ];
	return names[random.next() % names.Count];
}

function IsEqual : a, b
{
	return a == b;
}

function Fibonacci
{
	var n0 = 0, n1 = 1, naux;
	
	if (n == 0)
		return 0;
	
	Print("0 1 ");
	
	for(var i = 0; i < n - 1; ++i) {
		nause = n1;
		n1 = n0 + n1;
		n0 = nause;
		
		Print(n0.str);
	}
	
	return n1;
}

/*
 *	Oprhaned { } denotes entry point if this is the first script called
 *	This can be used for unit testing classes or methods and providing an entry point
 *	for a scripted program. This will NOT be called if loaded into the VM from external call.
 */
{
	var name;
	
	// Grab name of user
	name = GetLine("Please, sir, your name? ");
	
	// Print a nice greeting message
	PrintLine("Nice to meet you, " + name + "! My name is " + PickName());
	
	// Make call to another module
	XyzModule.AbcCall();
	
	// Check who's awesome
	if (IsEqual(name, "Zach"))
		PrintLine("I see you're pretty much the best person ever.");
	else
		PrintLine("Get out of here scum.");
	
	// Call fibanacci
	Fibonacci();
	
	// Create some objects
	cat = Cat();
	dog = Dog();
	
	repeat(4)
		cat.Speak();
	
	dog.Speak();
	
	// Some loops
	var i = 0;
	foreach(var a in [ 1, 2, "a", "b" ])
		PrintLine((++i).str + ". " + a.str);
	
	while(i--)
		Thread.Sleep(1);
	
	// Elegant while(true) or for(;;)
	loop
	{
		
	}
}
