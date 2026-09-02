using System.Collections.Generic;
using System.Linq;

public class Kong : Meld, IBlock
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

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 5;
	}
}