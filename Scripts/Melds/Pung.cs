using System;
using System.Collections.Generic;
using System.Linq;

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

	public Pung(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Pung)).Distinct();
	}

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