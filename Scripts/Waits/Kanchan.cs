using System.Collections.Generic;
using System.Linq;

public class Kanchan : Wait, IBlock
{
	public Kanchan(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Kanchan)).Distinct();
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi)
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		if (tile.RawRank > 2)
		{
			var lowerTiles = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank - 2)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var lowerTile in lowerTiles)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(lowerTile);
				yield return new MadeBlockContext(
					new Kanchan([lowerTile, tile]),
					remainingTiles
				);
			}
		}

		if (tile.RawRank < 8)
		{
			var upperTiles = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank + 2)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var upperTile in upperTiles)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(upperTile);
				yield return new MadeBlockContext(
					new Kanchan([tile, upperTile]),
					remainingTiles
				);
			}
		}
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 19;
	}
}