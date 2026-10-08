using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Melds;

/// <summary>
/// A meld made up of a quad of four of the same tile, such as 2 Zi, 2 Zi, 2 Zi, 2 Zi ("2222z").
/// </summary>
/// <remarks>
/// Also known as a "kan" in Japanese terms. In this library, that term is saved to mean the call a player makes to form
/// this meld. As always, a red five counts the same as a non-red five.
/// </remarks>
public class Kong : Meld, IBlock, IDowngradable<Pung>
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a kong.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the kong.</param>
	public Kong(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible kongs that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of kongs.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kongs that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Kong)).Distinct();
	}

	/// <summary>
	/// Get possible kongs that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created kongs.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kongs that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		var otherTilesList = otherTiles.ToList();
		var matchingTiles = otherTilesList.Where(t => t.RawEquals(tile)).ToList();
		var nonMatchingTiles = otherTilesList.Where(t => !t.RawEquals(tile)).ToList();

		if (matchingTiles.Count != 3)
		{
			yield break;
		}

		yield return new MadeBlockContext(
			new Kong(matchingTiles.Append(tile)),
			nonMatchingTiles
		);
	}

	/// <summary>
	/// Get the hash code basis representing a kong, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for kongs.</returns>
	public static new int GetHashCodeBasis()
	{
		return 5;
	}

	IEnumerable<MadeBlockContext> IDowngradable<Pung>.Downgrade()
	{
		var redFive = _tiles.SingleOrDefault(t => t.Rank == 0);
		if (redFive != default)
		{
			var nonRedFives = _tiles.Where(t => t.Rank == 5);
			yield return new MadeBlockContext(
				new Pung(nonRedFives.Take(2).Append(redFive)),
				nonRedFives.Skip(2)
			);
			yield return new MadeBlockContext(
				new Pung(nonRedFives),
				[redFive]
			);
		}
		else
		{
			yield return new MadeBlockContext(
				new Pung(_tiles.Take(3)),
				_tiles.Skip(3)
			);
		}
	}
}