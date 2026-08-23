using System.Collections.Generic;
using System.Linq;

public class MadeBlockContext
{
	private readonly Block _madeBlock;
	private readonly IEnumerable<Tile> _remainingTiles;

	public Block MadeBlock => _madeBlock;
	public IEnumerable<Tile> RemainingTiles => _remainingTiles;

	public MadeBlockContext(Block madeBlock, IEnumerable<Tile> remainingTiles)
	{
		_madeBlock = madeBlock;
		_remainingTiles = remainingTiles;
	}

	public override bool Equals(object that)
	{
		if ((that == null) ||
			(that is not MadeBlockContext thatContext) ||
			(_madeBlock != thatContext._madeBlock))
		{
			return false;
		}

		var sortedThis = _remainingTiles.Order().ToList();
		var sortedThat = thatContext._remainingTiles.Order().ToList();
		if (sortedThis.Count != sortedThat.Count)
		{
			return false;
		}

		for (var i = 0; i < sortedThis.Count; i++)
		{
			var currentThis = sortedThis[i];
			var currentThat = sortedThat[i];
			if (currentThis != currentThat)
			{
				return false;
			}
		}

		return true;
	}

	public override int GetHashCode()
	{
		var hashCodeBasis = _madeBlock.GetHashCode();
		var hashCodeExponent = 1;
		foreach (var tile in _remainingTiles)
		{
			hashCodeExponent = hashCodeExponent * hashCodeBasis + tile.GetHashCode();
		}

		// This is not guaranteed to be under max int, but our numbers are pretty low so I'm not that worried about it.
		return hashCodeBasis ^ hashCodeExponent;
	}
}