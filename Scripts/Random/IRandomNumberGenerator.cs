namespace Koffing.Random;

/// <summary>
/// Wrapper class for random number generators, providing a common interface.
/// </summary>
/// <remarks>
/// This class exists to expose the same interface for the Godot RNG
/// (<see cref="Godot.Random.GodotRandomNumberGenerator"/>) and the System.Random RNG
/// (<see cref="SystemRandomNumberGenerator"/>), and theoretically any other RNGs that users may want to use with the
/// library.
/// </remarks>
public interface IRandomNumberGenerator
{
	/// <summary>
	/// The seed value for the random number generator.
	/// </summary>
	/// <value>The <see langword="ulong"/> that was used to seed the random number generator.</value>
	ulong Seed { get; set; }

	/// <summary>
	/// Randomizes the random number generator, picking a seed value at random.
	/// </summary>
	void Randomize();

	/// <summary>
	/// Returns a random integer within the specified interval.
	/// </summary>
	/// <param name="from">The lower bound of the interval (inclusive).</param>
	/// <param name="to">The upper bound of the interval (inclusive).</param>
	/// <returns>
	/// A random integer within the range specified by <paramref name="from"/> and <paramref name="to"/>.
	/// </returns>
	int GetIntegerInRange(int from, int to);
}