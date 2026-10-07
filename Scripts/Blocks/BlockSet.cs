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

	/// <inheritdoc/>
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

	/// <inheritdoc/>
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
	public bool Equals(BlockSet that)
	{
		return CompareTo(that) == 0;
	}

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
	/// Defers to <see cref="Equals(BlockSet)"/> after converting <paramref name="obj"/> using the <c>as</c> keyword.
	/// </remarks>
	public override bool Equals(object obj)
	{
		return Equals(obj as BlockSet);
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