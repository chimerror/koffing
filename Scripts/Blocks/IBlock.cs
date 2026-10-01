using System.Collections.Generic;
using Koffing.Tiles;

namespace Koffing.Blocks;

public interface IBlock
{
	static abstract IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles);
	static abstract IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles);
	static abstract int GetHashCodeBasis();
}