using System;

public class SystemRandomNumberGenerator : IRandomNumberGenerator
{
	private Random _rng = new();
	private ulong _seed = 0ul;

	public SystemRandomNumberGenerator()
	{
		Randomize();
	}

	public ulong Seed
	{
		get => _seed;
		set
		{
			// Set to truncatedSeed, not value so that we don't imply any extra consideration of truncated high bits
			int truncatedSeed = (int)value;
			_seed = (ulong)truncatedSeed;
			_rng = new Random(truncatedSeed);
		}
	}

	public int GetIntegerInRange(int from, int to)
	{
		// To + 1 because System.Random doesn't include the upper bound. Could cause an overflow, but unlikely with the
		// type of numbers we're going to use.
		return _rng.Next(from, to + 1);
	}

	public void Randomize()
	{
		var tempSeed = _rng.Next();
		_seed = (ulong)tempSeed;
		_rng = new Random(tempSeed);
	}
}