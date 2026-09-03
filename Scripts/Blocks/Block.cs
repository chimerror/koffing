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

	public static IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		// This is commented out to force implementation, but child classes should override this method with something
		// like the below, replacing `typeof(Block)` with `typeof(ChildBlock)`.
		// return GetPossibleHelper(tiles, typeof(Block)).Distinct();

		throw new NotImplementedException();
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

	protected static IEnumerable<MadeBlockContext> GetPossibleHelper(IEnumerable<Tile> tiles, Type blockType)
	{
		var distinctTiles = tiles.DistinctBy(t => (t.Suit, t.Rank)).ToList();
		foreach (var tile in distinctTiles)
		{
			// TODO: Really don't understand why my ReferenceEquals implementation of this didn't work, but I'm tired
			// of fussing with it.
			var otherTiles = tiles.ToList();
			otherTiles.Remove(tile);

			var getPossibleForTileMethod = blockType.GetMethod(nameof(GetPossibleForTile));
			var possibleBlocks =
				((IEnumerable<MadeBlockContext>)getPossibleForTileMethod.Invoke(null, [tile, otherTiles]))
				.ToList();
			foreach (var madeBlock in possibleBlocks)
			{
				yield return madeBlock;
			}
		}
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
		unchecked
		{
			// WARNING: I am honestly unsure how good this hashing function is, or if it being bad will actually cause
			// problems. Just keep an eye out.
			var hashCodeBasis = GetActualHashCodeBasis();
			var hashCodeExponent = 1;
			foreach (var tile in _tiles)
			{
				hashCodeExponent = hashCodeExponent * hashCodeBasis + tile.GetHashCode();
			}

			// WARNING: Also worried a bit about the likely conversions to and from float here, but I don't think this
			// will be that used.
			return (int)Math.Pow(hashCodeBasis, hashCodeExponent);
		}
	}

	public IEnumerator<Tile> GetEnumerator()
	{
		return _tiles.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public int CompareTo(Block that)
	{
		if (that == null)
		{
			return 1;
		}

		// Because we check type here first, this means that blocks will first be arranged by type and then by their
		// tiles, unlike we'd probably do in a hand. This is OK for our purposes as an engine, but might be something
		// to work around when displaying it to the player. But this way we minimize the checking we have to do as
		// well as avoiding the iterations down below.
		if (GetType() != that.GetType())
		{
			var thisBasis = GetActualHashCodeBasis();
			var thatBasis = that.GetActualHashCodeBasis();
			return thisBasis.CompareTo(thatBasis);
		}

		// The length check here should never be triggered because blocks of the same type should also have the same
		// lengths (though nothing enforces that). I guess this is a fine defensive check, and I could also add variant
		// blocks perhaps in the future that could differ in lengths.
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