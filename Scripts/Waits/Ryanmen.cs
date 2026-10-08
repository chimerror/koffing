using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait made up of two tiles in the same non-Zi suit connected by adjacent ranks, without being on the edge like
/// a <see cref="Penchan"/>. For example 4 Sou and 5 Sou ("45s").
/// </summary>
/// <remarks>
/// Could be translated into English as "outside wait".
/// </remarks>
public class Ryanmen : Wait, IBlock
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a ryanmen.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the ryanmen.</param>
	public Ryanmen(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible ryanmens that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of ryanmens.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of ryanmens that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Ryanmen)).Distinct();
	}

	/// <summary>
	/// Get possible ryanmens that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created ryanmens.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of ryanmens that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi || tile.Rank == 1 || tile.Rank == 9)
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		if (tile.RawRank > 2)
		{
			var lowerNeighbors = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank - 1)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var lowerNeighbor in lowerNeighbors)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(lowerNeighbor);
				yield return new MadeBlockContext(
					new Ryanmen([lowerNeighbor, tile]),
					remainingTiles
				);
			}
		}

		if (tile.RawRank < 8)
		{
			var upperNeighbors = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank + 1)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var upperNeighbor in upperNeighbors)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(upperNeighbor);
				yield return new MadeBlockContext(
					new Ryanmen([tile, upperNeighbor]),
					remainingTiles
				);
			}
		}
	}

	/// <summary>
	/// Get the hash code basis representing a ryanmen, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for ryanmens.</returns>
	public static new int GetHashCodeBasis()
	{
		return 17;
	}
}