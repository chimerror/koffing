using System.Collections.Generic;

public interface IBlock
{
	static abstract IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles);
	static abstract int GetHashCodeBasis();
}