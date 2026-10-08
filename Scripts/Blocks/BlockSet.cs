using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Koffing.Blocks;

// TODO: What does this class look like when ToString is called? Bet it could be improved.

/// <summary>
/// A collection of Blocks, usually representing an entire blocked-out hand.
/// </summary>
public class BlockSet : IEnumerable<Block>, IComparable<BlockSet>, IEquatable<BlockSet>
{
	private readonly List<Block> _blocks;

	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Block"/>s.
	/// </summary>
	/// <param name="blocks">The <see cref="Block"/>s that make up the BlockSet.</param>
	public BlockSet(IEnumerable<Block> blocks = null)
	{
		if (blocks != null)
		{
			_blocks = [.. blocks];
		}
		else
		{
			_blocks = [];
		}
	}

	/// <summary>
	/// Indexer.
	/// </summary>
	/// <value>The <see cref="Block"/> at index <paramref name="index"/> in the BlockSet.</value>
	public Block this[int index]
	{
		get => _blocks[index];
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
	/// 		the number of blocks
	/// 	</item>
	/// 	<item>
	/// 		the actual blocks in the block-set (after sorting) using <see cref="Block.CompareTo(Block)"/>.
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="that">A <see cref="BlockSet"/> to compare with this instance.</param>
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
	public int CompareTo(BlockSet that)
	{
		if (that == null)
		{
			return 1;
		}

		var thisBlocks = _blocks.Order().ToList();
		var thatBlocks = that.Order().ToList();
		if (thisBlocks.Count != thatBlocks.Count)
		{
			return thisBlocks.Count.CompareTo(thatBlocks.Count);
		}

		for (var i = 0; i < thisBlocks.Count; i++)
		{
			var blockA = thisBlocks[i];
			var blockB = thatBlocks[i];
			if (!blockA.Equals(blockB))
			{
				return blockA.CompareTo(blockB);
			}
		}

		return 0;
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <remarks>
	/// Equality is determined by:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		the number of blocks
	/// 	</item>
	/// 	<item>
	/// 		the actual blocks in the block-set (after sorting) using <see cref="Block.Equals(Block)"/>.
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="that">The <see cref="BlockSet"/> to compare with the current object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="that"/> parameter; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public bool Equals(BlockSet that)
	{
		return CompareTo(that) == 0;
	}

	/// <inheritdoc/>
	public IEnumerator<Block> GetEnumerator()
	{
		return _blocks.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Defers to <see cref="Equals(BlockSet)"/> after converting <paramref name="that"/> using the
	/// <see langword="as"/> keyword.
	/// </remarks>
	/// <param name="that">The object to compare with the current object.</param>
	public override bool Equals(object that)
	{
		return Equals(that as BlockSet);
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Hash code is calculated as the multiple of the hash codes of all blocks using <see cref="Block.GetHashCode"/>.
	/// Given the design of the block hash code based on multiples of exponentiated prime numbers, this should be have
	/// pretty good hashing (but this has not been verified).
	/// </remarks>
	/// <seealso cref="Block.GetHashCode"/>
	public override int GetHashCode()
	{
		unchecked
		{
			// Should be pretty unique because each block should be its basis to a power, so should get a big string
			// of the bases to exponents multiplied together.
			var hashCode = 1;
			foreach (var block in _blocks)
			{
				hashCode *= block.GetHashCode();
			}
			return hashCode;
		}
	}
}