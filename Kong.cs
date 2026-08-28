using System.Collections.Generic;
using System.Linq;

public class Kong : Block, IBlock
{
	public Kong(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		var otherTilesList = otherTiles.ToList();
		var matchingTiles = otherTilesList.Where(t => t.RawEquals(tile)).ToList();
		var nonMatchingTiles = otherTilesList.Where(t => !t.RawEquals(tile)).ToList();

		if (matchingTiles.Count != 3)
		{
			yield break;
		}

		yield return new MadeBlockContext(
			new Kong(matchingTiles.Append(tile)),
			nonMatchingTiles
		);
	}
}