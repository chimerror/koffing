using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Waits;

// TODO: Since I ended up using English terms most everywhere, I am considering using them for these waits as well. I
// do not think it would make as much sense to do that with the basic melds though. But this is a very minor style
// thing.

/// <summary>
/// A wait made up of two tiles in the same non-Zi suit two ranks apart, such as 6 Pin and 8 Pin ("68p").
/// </summary>
/// <remarks>
/// Could be translated into English as an "inside wait".
/// </remarks>
public class Kanchan : Wait, IBlock
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a kanchan.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the kanchan.</param>
	public Kanchan(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible kanchans that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of kanchans.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kanchans that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Kanchan)).Distinct();
	}

	/// <summary>
	/// Get possible kanchans that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created kanchans.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kanchans that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi)
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		if (tile.RawRank > 2)
		{
			var lowerTiles = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank - 2)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var lowerTile in lowerTiles)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(lowerTile);
				yield return new MadeBlockContext(
					new Kanchan([lowerTile, tile]),
					remainingTiles
				);
			}
		}

		if (tile.RawRank < 8)
		{
			var upperTiles = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank + 2)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var upperTile in upperTiles)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(upperTile);
				yield return new MadeBlockContext(
					new Kanchan([tile, upperTile]),
					remainingTiles
				);
			}
		}
	}

	/// <summary>
	/// Get the hash code basis representing a kanchan, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for kanchans.</returns>
	public static new int GetHashCodeBasis()
	{
		return 19;
	}
}