using System.Collections.Generic;
using System.Linq;

public class Kong : Meld, IBlock, IDowngradable<Pung>
{
	public Kong(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Kong)).Distinct();
	}

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