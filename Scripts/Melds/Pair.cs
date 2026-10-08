using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Melds;

/// <summary>
/// A meld made up of a pair of two of the same tile such as 7 Pin, 7 Pin ("77p").
/// </summary>
/// <remarks>
/// The difference between this and <see cref="Waits.PairWait"/> is merely one of semantic intent, and this class can
/// be explicitly cast to a <see cref="Waits.PairWait"/> as needed. As always, a red five counts the same as a non-red
/// five.
/// </remarks>
public class Pair : Meld, IBlock
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a pair.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the pair.</param>
	public Pair(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible pairs that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of pairs.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pairs that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Pair)).Distinct();
	}

	/// <summary>
	/// Get possible pairs that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created pairs.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pairs that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	// TODO: Right now we assume only one red 5, which may not be true in the future. This code will have to be
	// updated if that changes.
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		var otherTilesList = otherTiles.ToList();
		var matchingTiles = otherTilesList.Where(t => t.RawEquals(tile)).ToList();
		var nonMatchingTiles = otherTilesList.Where(t => !t.RawEquals(tile)).ToList();

		if (matchingTiles.Count < 1)
		{
			yield break;
		}

		if (tile.Suit != Suit.Zi && tile.Rank == 5 && matchingTiles.Any(t => t.Rank == 0))
		{
			var matchingRedFive = matchingTiles.First(t => t.Rank == 0);
			var matchingFives = matchingTiles.Where(t => t.Rank == 5).ToList();

			yield return new MadeBlockContext(
				new Pair([tile, matchingRedFive]),
				nonMatchingTiles.Concat(matchingFives)
			);

			if (matchingFives.Count > 0)
			{
				var firstMatchingFive = matchingFives.First();
				yield return new MadeBlockContext(
					new Pair([tile, firstMatchingFive]),
					nonMatchingTiles.Concat(matchingFives.Skip(1)).Append(matchingRedFive)
				);
			}
		}
		else
		{
			yield return new MadeBlockContext(
				new Pair(matchingTiles.Take(1).Append(tile)),
				nonMatchingTiles.Concat(matchingTiles.Skip(1))
			);
		}
	}

	/// <summary>
	/// Get the hash code basis representing a pair, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for pairs.</returns>
	public static new int GetHashCodeBasis()
	{
		return 7;
	}
}