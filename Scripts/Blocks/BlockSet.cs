using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BlockSet : IEnumerable<Block>, IComparable<BlockSet>, IEquatable<BlockSet>
{
	private readonly List<Block> _blocks;

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

	public Block this[int index]
	{
		get => _blocks[index];
	}

	public IEnumerable<Block> Blocks
	{
		get => _blocks.AsReadOnly();
	}

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

	public override bool Equals(object obj)
	{
		return Equals(obj as BlockSet);
	}

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