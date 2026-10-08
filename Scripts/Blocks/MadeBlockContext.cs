using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Tiles;

namespace Koffing.Blocks;

/// <summary>
/// Represents a "made" block, a pair of a <see cref="Block"/> and the <see cref="Tile"/>s that remain after making the
/// block.
/// </summary>
/// <remarks>
/// By wrapping up these two values, we have an easy way to recursively generate blocks from a collection of tiles, by
/// recursing on the <see cref="RemainingTiles"/> property.
/// </remarks>
public class MadeBlockContext : IComparable<MadeBlockContext>, IEquatable<MadeBlockContext>
{
	private readonly Block _madeBlock;
	private readonly IEnumerable<Tile> _remainingTiles;

	/// <summary>
	/// The <see cref="Block"/> that was formed for this particular context.
	/// </summary>
	public Block MadeBlock => _madeBlock;

	/// <summary>
	/// An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s that are the tiles that were left over after forming the
	/// block returned by <see cref="MadeBlock"/>.
	/// </summary>
	public IEnumerable<Tile> RemainingTiles => _remainingTiles;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="madeBlock">The <see cref="Block"/> that was formed for this particular context.</param>
	/// <param name="remainingTiles">
	/// An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s representing the tiles that were left over after forming
	/// <paramref name="madeBlock"/>.
	/// </param>
	public MadeBlockContext(Block madeBlock, IEnumerable<Tile> remainingTiles)
	{
		_madeBlock = madeBlock;
		_remainingTiles = remainingTiles;
	}

	// TODO: There should probably be a set order for these common functions like Equals and CompareTo.

	/// <inheritdoc/>
	/// <remarks>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		if <paramref name="that"/> is a <see cref="MadeBlockContext"/>
	/// 	</item>
	/// 	<item>
	/// 		then deferring to <see cref="Equals(MadeBlockContext)"/>
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="that">The object to compare with the current object.</param>
	public override bool Equals(object that)
	{
		if ((that == null) ||
			(that is not MadeBlockContext thatContext))
		{
			return false;
		}

		return Equals(thatContext);
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <remarks>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="thatContext"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		the <see cref="MadeBlock"/> using <see cref="Block.Equals(Block)"/>
	/// 	</item>
	/// 	<item>
	/// 		the count of tiles in <see cref="RemainingTiles"/>
	/// 	</item>
	/// 	<item>
	/// 		the tiles in <see cref="RemainingTiles"/> (after sorting)
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="thatContext">The <see cref="MadeBlockContext"/> to compare with the current object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="thatContext"/> parameter;
	/// otherwise, <see langword="false"/>.
	/// </returns>
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

	/// <inheritdoc/>
	/// <remarks>
	/// Calculates hash value using the value returned from <see cref="IBlock.GetHashCodeBasis"/> for
	/// <see cref="MadeBlock"/> as a basis which is combined using bitwise exclusive or with an "exponent" calculated by
	/// that same basis and the hash code of the tiles in <see cref="RemainingTiles"/> using
	/// <see cref="Tile.GetHashCode"/>.
	/// </remarks>
	public override int GetHashCode()
	{
		var hashCodeBasis = _madeBlock.GetHashCode();
		var hashCodeExponent = 1;
		foreach (var tile in _remainingTiles)
		{
			hashCodeExponent = hashCodeExponent * hashCodeBasis + tile.GetHashCode();
		}

		// TODO: This isn't what we should be using for exponentiation; it's the bitwise xor operator. However, in this
		// case I'm not sweating using xor here unlike in Block, where I _really_ want to make sure to make blocks
		// uniquely hash based on type.
		// This is not guaranteed to be under max int, but our numbers are pretty low so I'm not that worried about it.
		return hashCodeBasis ^ hashCodeExponent;
	}

	/// <summary>
	/// Compares the current instance with another object of the same type and returns an integer that indicates whether
	/// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <remarks>
	/// Comparison is done by comparing:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always 1)
	/// 	</item>
	/// 	<item>
	/// 		if the <see cref="MadeBlock"/>s aren't equal and then using <see cref="Block.CompareTo(Block)"/> on
	/// 		them if not.
	/// 	</item>
	/// 	<item>
	/// 		the count of tiles in <see cref="RemainingTiles"/>
	/// 	</item>
	/// 	<item>
	/// 		the tiles in <see cref="RemainingTiles"/> using <see cref="Tile.CompareTo(Tile)"/> (after sorting)
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="that">A <see cref="MadeBlockContext"/> to compare with this instance.</param>
	/// <returns>
	/// A value that indicates the relative order of the objects being compared. The return value has these meanings:
	/// <list type="table">
	/// 	<listheader>
	/// 		<term>Value</term>
	/// 		<description>Meaning</description>
	/// 	</listheader>
	/// 	<item>
	/// 		<term>Less than zero</term>
	/// 		<description>This instance precedes <paramref name="that"/> in the sort order.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Zero</term>
	/// 		<description>
	/// 			This instance occurs in the same position in the sort order as <paramref name="that"/>.
	/// 		</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Greater than zero</term>
	/// 		<description>This instance follows <paramref name="that"/> in the sort order.</description>
	/// 	</item>
	/// </list>
	/// </returns>
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

	/// <inheritdoc/>
	/// <remarks>
	/// Prints:
	/// <list type="number">
	/// 	<item>
	/// 		<c>"MadeBlockContext "</c>
	/// 	</item>
	/// 	<item>
	/// 		the name of the <see cref="Type"/> of <see cref="MadeBlock"/>
	/// 	</item>
	/// 	<item>
	/// 		<c>": "</c>
	/// 	</item>
	/// 	<item>
	/// 		the MPSZ notation of <see cref="MadeBlock"/> as generated by
	/// 		<see cref="Extensions.NotationFromTiles(IEnumerable{Tile})"/>.
	/// 	</item>
	/// 	<item>
	/// 		<c>", "</c>
	/// 	</item>
	/// 	<item>
	/// 		the MPSZ notation of <see cref="RemainingTiles"/> as generated by
	/// 		<see cref="Extensions.NotationFromTiles(IEnumerable{Tile})"/>.
	/// 	</item>
	/// </list>
	/// </remarks>
	public override string ToString()
	{
		return $"MadeBlockContext {_madeBlock.GetType().Name}: {_madeBlock.NotationFromTiles()}, {_remainingTiles.NotationFromTiles()}";
	}
}