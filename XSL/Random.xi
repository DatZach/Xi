/*
 *	Random.xi
 *	Xi Standard Library
 *		Random
 */

class Random
{
	private const RANDOM_MAX = 2147483648.0;
	
	private var seed;
	private var next;
	
	public constructor
	{
		// TODO Use time as the initial seed
		SetSeed(1234321.0);
	}
	
	public function SetSeed : s
	{
		seed = s;
		next = s;
	}
	
	public function Random
	{
		next = (next * 1103515245.0 + 12345.0) % (RANDOM_MAX + 1.0);
		return next;
	}
	
	public function Random : max
	{
		return Random() % max;
	}
	
	public function Random : min, max
	{
		var value = Random();
		return (value % (max - min)) + min;
	}
	
	public function RandomInteger
	{
		return int(Random());
	}
}
