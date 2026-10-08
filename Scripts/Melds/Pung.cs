using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Melds;

/// <summary>
/// A meld made up of a triplet of three of the same tile such as 3 Man, 3 Man, 3 Man ("333m").
/// </summary>
/// <remarks>
/// Also known as a "pon" in Japanese terms. In this library, that term is saved to mean the call a player makes to
/// take a discard and form this meld. As always, a red five counts the same as a non-red five.
/// </remarks>
public class Pung : Meld, IBlock, IDowngradable<Pair>, IUpgradable<Kong>
{
	HashSet<Tile> IUpgradable<Kong>.SoughtTiles
	{
		get
		{
			var suit = _tiles[0].Suit;
			var rank = _tiles[0].RawRank;
			if (suit != Suit.Zi && rank == 5)
			{
				var redFive = _tiles.SingleOrDefault(t => t.Rank == 0);
				var nonRedFives = _tiles.Where(t => t.Rank != 0);
				var soughtTiles = new HashSet<Tile>();
				if (redFive == default)
				{
					soughtTiles.Add(new Tile(suit, 0));
				}
				else
				{
					soughtTiles.Add(new Tile(suit, 5));
				}
				return soughtTiles;
			}
			else
			{
				return new HashSet<Tile>([_tiles[0]]);
			}
		}
	}

	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a pung.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the pung.</param>
	public Pung(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible pungs that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of pungs.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pungs that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Pung)).Distinct();
	}

	/// <summary>
	/// Get possible pungs that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created pungs.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of pungs that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	// TODO: Right now we assume only one red 5, which may not be true in the future. This code will have to be
	// updated if that changes.
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		var otherTilesList = otherTiles.ToList();
		var matchingTiles = otherTilesList.Where(t => t.RawEquals(tile)).ToList();
		var nonMatchingTiles = otherTilesList.Where(t => !t.RawEquals(tile)).ToList();

		if (matchingTiles.Count < 2)
		{
			yield break;
		}

		if (tile.Suit != Suit.Zi && tile.Rank == 5 && matchingTiles.Count == 3)
		{
			var matchingRedFive = matchingTiles.Single(t => t.Rank == 0);
			var matchingFives = matchingTiles.Where(t => t.Rank == 5);

			yield return new MadeBlockContext(
				new Pung(matchingFives.Take(2).Append(tile)),
				nonMatchingTiles.Append(matchingRedFive)
			);
			yield return new MadeBlockContext(
				new Pung(matchingFives.Take(1).Append(matchingRedFive).Append(tile)),
				nonMatchingTiles.Concat(matchingFives.Skip(1))
			);
		}
		else
		{
			if (matchingTiles.Count == 3)
			{
				nonMatchingTiles.Add(matchingTiles.Last());
			}
			yield return new MadeBlockContext(
				new Pung(matchingTiles.Take(2).Append(tile)),
				nonMatchingTiles
			);
		}
	}

	/// <summary>
	/// Get the hash code basis representing a pung, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for pungs.</returns>
	public static new int GetHashCodeBasis()
	{
		return 3;
	}

	IEnumerable<MadeBlockContext> IDowngradable<Pair>.Downgrade()
	{
		var redFive = _tiles.SingleOrDefault(t => t.Rank == 0);
		if (redFive != default)
		{
			var nonRedFives = _tiles.Where(t => t.Rank == 5);
			yield return new MadeBlockContext(
				new Pair(nonRedFives.Take(1).Append(redFive)),
				nonRedFives.Skip(1)
			);
			yield return new MadeBlockContext(
				new Pair(nonRedFives),
				[redFive]
			);
		}
		else
		{
			yield return new MadeBlockContext(
				new Pair(_tiles.Take(2)),
				_tiles.Skip(2)
			);
		}
	}

	bool IUpgradable<Kong>.CanUpgradeWith(Tile tile)
	{
		return tile.Suit == _tiles[0].Suit && tile.RawRank == _tiles[0].RawRank;
	}

	Kong IUpgradable<Kong>.Upgrade(Tile tile)
	{
		if (!((IUpgradable<Kong>)this).CanUpgradeWith(tile))
		{
			throw new ArgumentException($"Attempted to upgrade kong of {_tiles[0]} with invalid tile {tile}");
		}

		return new Kong(_tiles.Append(tile));
	}
}