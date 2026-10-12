using Godot;
using Koffing.Random;

namespace Koffing.Godot.Random;

/// <summary>
/// An implementation of the <see cref="IRandomNumberGenerator"/> interface wrapping around the Godot random number
/// generator.
/// </summary>
public class GodotRandomNumberGenerator : IRandomNumberGenerator
{
	private readonly RandomNumberGenerator _rng = new();

	/// <summary>
	/// Parameterless constructor. Chooses a seed at random as a result of the
	/// <see href="https://docs.godotengine.org/en/4.7/classes/class_randomnumbergenerator.html#class-randomnumbergenerator">
	/// Godot RandomNumberGenerator
	/// </see>
	/// constructor.
	/// </summary>
	public GodotRandomNumberGenerator()
	{
	}

	/// <inheritdoc/>
	public ulong Seed { get => _rng.Seed; set => _rng.Seed = value; }

	/// <inheritdoc/>
	public void Randomize()
	{
		_rng.Randomize();
	}

	/// <inheritdoc/>
	public int GetIntegerInRange(int from, int to)
	{
		return _rng.RandiRange(from, to);
	}
}