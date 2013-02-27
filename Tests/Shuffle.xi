/*
 *	Shuffle.xi
 *	Test XSL Random
 */

using Random;

// Fisher-Yates shuffle
function Shuffle : set
{
	var rng = new Random.Random();
	var n = len set;
	
	while(n > 1)
	{
		--n;
		
		var k = rng.Random(n + 1);
		var value = set[k];
		set[k] = set[n];
		set[n] = value;
	}
	
	return set;
}

{
	// Initialize a linear list
	var initial = [ 0 .. 16 ];
	for(var i = 0; i < 16; ++i)
		initial[i] = i;
	
	// Print shuffled result
	print Shuffle(initial);
}
