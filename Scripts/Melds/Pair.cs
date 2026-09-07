using System.Collections.Generic;
using System.Linq;

public class Pair : Meld, IBlock
{
	public Pair(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Pair)).Distinct();
	}

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

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 7;
	}
}