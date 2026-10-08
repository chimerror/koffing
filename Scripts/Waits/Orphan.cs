using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait made up of a single tile, unconnected from all others.
/// </summary>
public class Orphan : Wait
{
	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used, and is only defined for testing purposes. Rather, use
	/// <see cref="Orphan(Tile)"/>, so that there is a tile associated with the class, maintaining the semantic meaning
	/// of an orphan.
	/// </remarks>
	public Orphan() : base()
	{
	}

	/// <summary>
	/// Constructor taking a <see cref="Tile"/>.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> to mark as an orphan.</param>
	public Orphan(Tile tile) : base([tile])
	{
	}

	/// <summary>
	/// Get the hash code basis representing a orphan, which will be exponentiated as a part of calculating
	/// <see cref="Blocks.Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for orphans.</returns>
	public static new int GetHashCodeBasis()
	{
		return 11;
	}
}