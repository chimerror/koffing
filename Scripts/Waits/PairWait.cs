using System.Collections.Generic;
using System.Linq;

public class PairWait : Wait, IBlock
{
	public PairWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static explicit operator Pair(PairWait pairWait) => new(pairWait.Tiles);

	public static explicit operator PairWait(Pair pair) => new(pair.Tiles);

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(PairWait)).Distinct();
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		return Pair.GetPossibleForTile(tile, otherTiles)
			.Select(c => new MadeBlockContext(new PairWait(c.MadeBlock.Tiles), c.RemainingTiles));
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 13;
	}
}