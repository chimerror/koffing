using System.Collections.Generic;
using System.Linq;

public class Chow : Meld, IBlock
{
	public Chow(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi)
		{
			yield break;
		}

		var firstStartingRank = tile.RawRank - 2;
		if (tile.RawRank <= 2)
		{
			firstStartingRank = 1;
		}
		else if (tile.RawRank > 8)
		{
			firstStartingRank = 7;
		}
		var lastStartingRank = tile.RawRank > 7 ? 7 : tile.RawRank;

		var otherTilesList = otherTiles.ToList();
		for (var startingRank = firstStartingRank; startingRank <= lastStartingRank; startingRank++)
		{
			var lowTiles = GetRankTiles(startingRank, tile, otherTilesList).ToList();
			var middleTiles = GetRankTiles(startingRank + 1, tile, otherTilesList).ToList();
			var highTiles = GetRankTiles(startingRank + 2, tile, otherTilesList).ToList();

			if (lowTiles.Count == 0 || middleTiles.Count == 0 || highTiles.Count == 0)
			{
				continue;
			}

			foreach (var lowTile in lowTiles)
			{
				foreach (var middleTile in middleTiles)
				{
					foreach (var highTile in highTiles)
					{
						yield return new MadeBlockContext(
							new Chow([lowTile, middleTile, highTile]),
							otherTilesList.Where(t => NotChosen(t, lowTile, middleTile, highTile))
						);
					}
				}
			}
		}
	}

	private static IEnumerable<Tile> GetRankTiles(int desiredRank, Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.RawRank == desiredRank)
		{
			yield return tile;
		}
		else
		{
			var matchingTiles = otherTiles
				.Where(t => t.Suit == tile.Suit && t.RawRank == desiredRank)
				.GroupBy(t => t.Rank)
				.Select(g => g.First());
			foreach (var matchingTile in matchingTiles)
			{
				yield return matchingTile;
			}
		}
	}

	private static bool NotChosen(Tile candidateTile, Tile lowTile, Tile middleTile, Tile highTile)
	{
		return !(ReferenceEquals(candidateTile, lowTile) || ReferenceEquals(candidateTile, middleTile) || ReferenceEquals(candidateTile, highTile));
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 2;
	}
}