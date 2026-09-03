using System.Collections.Generic;

public interface IBlock
{
	static abstract IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles);
	static abstract IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles);
	static abstract int GetHashCodeBasis();
}