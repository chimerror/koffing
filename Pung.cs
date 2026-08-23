using System.Collections.Generic;
using System.Linq;

public class Pung : Block, IBlock
{
	public Pung(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

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
				matchingFives.TakeLast(2)
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
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 3;
	}
}