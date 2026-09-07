using System.Collections.Generic;
using System.Linq;

public class Ryanmen : Wait, IBlock
{
	public Ryanmen(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Ryanmen)).Distinct();
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi || tile.Rank == 1 || tile.Rank == 9)
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		if (tile.RawRank > 2)
		{
			var lowerNeighbors = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank - 1)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var lowerNeighbor in lowerNeighbors)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(lowerNeighbor);
				yield return new MadeBlockContext(
					new Ryanmen([lowerNeighbor, tile]),
					remainingTiles
				);
			}
		}

		if (tile.RawRank < 8)
		{
			var upperNeighbors = otherTiles.Where(t => t.Suit == tile.Suit && t.RawRank == tile.RawRank + 1)
				.GroupBy(t => t.Rank)
				.Select(g => g.First())
				.ToList();
			foreach (var upperNeighbor in upperNeighbors)
			{
				var remainingTiles = new List<Tile>(otherTilesList);
				remainingTiles.Remove(upperNeighbor);
				yield return new MadeBlockContext(
					new Ryanmen([tile, upperNeighbor]),
					remainingTiles
				);
			}
		}
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 17;
	}
}