/*
 *	iexample.xi
 *	Implementation of example module
 */

import local.example;

module IExample.*;

func EntryPoint(string[] args) : void
{
	Cat cat = new Cat();
	Dog dog = new Dog();
	
	print "Animals Created: " + GetAnimalsCreated();
}