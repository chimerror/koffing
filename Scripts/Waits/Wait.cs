using System.Collections.Generic;
using System.Linq;

public abstract class Wait : Block
{
	public static IEnumerable<MadeBlockContext> GetFirstLevelWaits(IEnumerable<Tile> tiles)
	{
		// TODO: Need to make wait checks for seven pairs and thirteen orphans
		return PairWait.GetPossible(tiles)
			.Concat(Ryanmen.GetPossible(tiles))
			.Concat(Kanchan.GetPossible(tiles))
			.Concat(Penchan.GetPossible(tiles));
	}

	public Wait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}
}