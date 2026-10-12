using S = System;

namespace Koffing.Random;

/// <summary>
/// An implementation of the <see cref="IRandomNumberGenerator"/> interface wrapping around the
/// <see cref="S.Random">System.Random</see> random number generator.
/// </summary>
public class SystemRandomNumberGenerator : IRandomNumberGenerator
{
	private S.Random _rng = new();
	private ulong _seed = 0ul;

	/// <summary>
	/// Parameterless constructor. Chooses a seed at random using <see cref="Randomize"/>.
	/// </summary>
	public SystemRandomNumberGenerator()
	{
		Randomize();
	}

	/// <inheritdoc/>
	/// <remarks>
	/// The System.Random number generator uses an <see langword="int"/> as the seed rather than a
	/// <see langword="ulong"/>. Thus, when setting the seed using a <see langword="ulong"/>, the value will be
	/// truncated to fit in the range of <see langword="int"/>.
	/// </remarks>
	public ulong Seed
	{
		get => _seed;
		set
		{
			// Set to truncatedSeed, not value so that we don't imply any extra consideration of truncated high bits
			int truncatedSeed = (int)value;
			_seed = (ulong)truncatedSeed;
			_rng = new S.Random(truncatedSeed);
		}
	}

	/// <inheritdoc/>
	public int GetIntegerInRange(int from, int to)
	{
		// To + 1 because System.Random doesn't include the upper bound. Could cause an overflow, but unlikely with the
		// type of numbers we're going to use.
		return _rng.Next(from, to + 1);
	}

	/// <inheritdoc/>
	public void Randomize()
	{
		var tempSeed = _rng.Next();
		_seed = (ulong)tempSeed;
		_rng = new S.Random(tempSeed);
	}
}