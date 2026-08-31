using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Block : IEnumerable<Tile>, IBlock, IComparable<Block>, IEquatable<Block>
{
	private readonly List<Tile> _tiles;

	public Block(IEnumerable<Tile> tiles = null)
	{
		if (tiles != null)
		{
			_tiles = [.. tiles];
		}
		else
		{
			_tiles = [];
		}
	}

	public Tile this[int index]
	{
		get => _tiles[index];
	}

	public static IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		// Throwing because we want to force overriding
		throw new NotImplementedException();
	}

	public static int GetHashCodeBasis()
	{
		// Throwing because we want to force overriding
		throw new NotImplementedException();
	}

	public override bool Equals(object that)
	{
		if ((that == null) ||
			(GetType() != that.GetType()) ||
			(that is not Block thatBlock))
		{
			return false;
		}

		return Equals(thatBlock);
	}

	public bool Equals(Block thatBlock)
	{
		if (thatBlock == null)
		{
			return false;
		}

		if ((GetType() != thatBlock.GetType()) ||
			(_tiles.Count != thatBlock._tiles.Count))
		{
			return false;
		}

		var sortedThis = _tiles.Order().ToList();
		var sortedThat = thatBlock._tiles.Order().ToList();
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
		var hashCodeBasis = GetActualHashCodeBasis();
		var hashCodeExponent = 1;
		foreach (var tile in _tiles)
		{
			hashCodeExponent = hashCodeExponent * hashCodeBasis + tile.GetHashCode();
		}

		// This is not guaranteed to be under max int, but our numbers are pretty low so I'm not that worried about it.
		return hashCodeBasis ^ hashCodeExponent;
	}

	public IEnumerator<Tile> GetEnumerator()
	{
		return _tiles.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// TODO: Write tests for this
	public int CompareTo(Block that)
	{
		if (that == null)
		{
			return 1;
		}

		if (GetType() != that.GetType())
		{
			var thisBasis = GetActualHashCodeBasis();
			var thatBasis = that.GetActualHashCodeBasis();
			return thisBasis.CompareTo(thatBasis);
		}

		var thisTiles = this.Order().ToArray();
		var thatTiles = that.Order().ToArray();
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
		return base.ToString() + ": " + this.NotationFromTiles();
	}

	private int GetActualHashCodeBasis()
	{
		var blockType = GetType();
		var hashCodeBasisMethod = blockType.GetMethod(nameof(GetHashCodeBasis));
		return (int)hashCodeBasisMethod.Invoke(null, null);
	}
}