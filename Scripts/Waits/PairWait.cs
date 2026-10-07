using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Waits;

public class PairWait : Wait, IBlock
{
	public PairWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static explicit operator Pair(PairWait pairWait) => new(pairWait);

	public static explicit operator PairWait(Pair pair) => new(pair);

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(PairWait)).Distinct();
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		return Pair.GetPossibleForTile(tile, otherTiles)
			.Select(c => new MadeBlockContext(new PairWait(c.MadeBlock), c.RemainingTiles));
	}

	public static new int GetHashCodeBasis()
	{
		return 13;
	}
}