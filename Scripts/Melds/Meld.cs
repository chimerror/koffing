using System.Collections.Generic;

public abstract class Meld : Block
{
	public Meld(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}
}