using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait made up of a pair of two of the same tile such as 6 Zi, 6 Zi ("66z").
/// </summary>
/// <remarks>
/// The difference between this and <see cref="Pair"/> is merely one of semantic intent, and this class defers much of
/// its implementation to <see cref="Pair"/> to avoid duplication. As always, a red five counts the same as a non-red
/// five.
/// </remarks>
public class PairWait : Wait, IBlock
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a pair wait.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the pair wait.</param>
	public PairWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// An explicit cast from <see cref="PairWait"/> to <see cref="Pair"/>.
	/// </summary>
	/// <param name="pairWait">The <see cref="PairWait"/> to cast to a <see cref="Pair"/></param>
	public static explicit operator Pair(PairWait pairWait) => new(pairWait);

	/// <summary>
	/// An explicit cast from <see cref="Pair"/> to <see cref="PairWait"/>.
	/// </summary>
	/// <param name="pair">The <see cref="Pair"/> to cast to a <see cref="PairWait"/></param>
	public static explicit operator PairWait(Pair pair) => new(pair);

	/// <summary>
	/// Get possible pair waits that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of pair waits.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pair waits that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(PairWait)).Distinct();
	}

	/// <summary>
	/// Get possible pair waits that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created pair waits.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pair waits that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		return Pair.GetPossibleForTile(tile, otherTiles)
			.Select(c => new MadeBlockContext(new PairWait(c.MadeBlock), c.RemainingTiles));
	}

	/// <summary>
	/// Get the hash code basis representing a pair wait, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for pair waits.</returns>
	public static new int GetHashCodeBasis()
	{
		return 13;
	}
}