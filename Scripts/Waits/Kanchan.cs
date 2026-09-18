using System.Collections.Generic;
using System.Linq;

// TODO: Since I ended up using English terms most everywhere, I am considering using them for these waits as well. I
// do not think it would make as much sense to do that with the basic melds though. But this is a very minor style
// thing.
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
		return 19;
	}
}