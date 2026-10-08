using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait made up of either an 1-2 or an 8-9 of a non-Zi suit, such as 8 Man and 9 Man ("89m").
/// </summary>
/// <remarks>
/// Could be translated into English as an "edge wait".
/// </remarks>
public class Penchan : Wait, IBlock
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a penchan.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the penchan.</param>
	public Penchan(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible penchans that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of penchans.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of penchans that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Penchan)).Distinct();
	}

	/// <summary>
	/// Get possible penchans that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created penchans.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of penchans that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi || (tile.RawRank > 2 && tile.RawRank < 8))
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		var partnerRank = tile.RawRank switch
		{
			1 => 2,
			2 => 1,
			8 => 9,
			9 => 8,
			_ => throw new InvalidOperationException($"Tried to make a penchan with invalid rank {tile.RawRank}"),
		};

		var partners = otherTilesList.Where(t => t.Suit == tile.Suit && t.RawRank == partnerRank).ToList();
		var nonPartners = otherTilesList.Where(t => t.Suit != tile.Suit || t.RawRank != partnerRank).ToList();

		if (partners.Count > 0)
		{
			yield return new MadeBlockContext(
				new Penchan(partners.Take(1).Append(tile)),
				nonPartners.Concat(partners.Skip(1))
			);
		}
	}

	/// <summary>
	/// Get the hash code basis representing a penchan, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for penchans.</returns>
	public static new int GetHashCodeBasis()
	{
		return 23;
	}
}