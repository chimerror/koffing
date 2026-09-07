using System.Collections.Generic;

public abstract class Wait : Block
{
	public Wait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}
}