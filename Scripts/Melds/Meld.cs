using System.Collections.Generic;
using System.Linq;

public abstract class Meld : Block
{
	public static IEnumerable<MadeBlockContext> GetFirstLevelMelds(IEnumerable<Tile> tiles)
	{
		return Chow.GetPossible(tiles)
			.Concat(Pung.GetPossible(tiles))
			.Concat(Kong.GetPossible(tiles));
	}

	public Meld(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}
}