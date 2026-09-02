using System;
using System.Collections.Generic;
using System.Linq;

public class MadeBlockContext : IComparable<MadeBlockContext>, IEquatable<MadeBlockContext>
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
			(that is not MadeBlockContext thatContext))
		{
			return false;
		}

		return Equals(thatContext);
	}

	public bool Equals(MadeBlockContext thatContext)
	{
		if (thatContext == null)
		{
			return false;
		}

		if (!_madeBlock.Equals(thatContext._madeBlock))
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
			if (!currentThis.Equals(currentThat))
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

	// TODO: Write tests for this
	public int CompareTo(MadeBlockContext that)
	{
		if (that == null)
		{
			return 1;
		}

		if (!MadeBlock.Equals(that.MadeBlock))
		{
			return MadeBlock.CompareTo(that.MadeBlock);
		}

		var thisTiles = RemainingTiles.Order().ToArray();
		var thatTiles = that.RemainingTiles.Order().ToArray();
		if (thisTiles.Length != thatTiles.Length)
		{
			return thisTiles.Length.CompareTo(thatTiles.Length);
		}

		for (int i = 0; i < thisTiles.Length; i++)
		{
			var thisTile = thisTiles[i];
			var thatTile = thatTiles[i];
			if (thisTile != thatTile)
			{
				return thisTile.CompareTo(thatTile);
			}
		}

		return 0;
	}

	public override string ToString()
	{
		return $"MadeBlockContext {_madeBlock.GetType().Name}: {_madeBlock.NotationFromTiles()}, {_remainingTiles.NotationFromTiles()}";
	}

}