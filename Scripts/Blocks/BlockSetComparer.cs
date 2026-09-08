using System.Collections.Generic;
using System.Linq;

public class BlockSetComparer : Comparer<IEnumerable<Block>>
{
	public override int Compare(IEnumerable<Block> blockSetA, IEnumerable<Block> blockSetB)
	{
		if (blockSetA == null)
		{
			return -1;
		}

		if (blockSetB == null)
		{
			return 1;
		}

		var blockSetListA = blockSetA.Order().ToList();
		var blockSetListB = blockSetB.Order().ToList();
		if (blockSetListA.Count != blockSetListB.Count)
		{
			return blockSetListA.Count.CompareTo(blockSetListB.Count);
		}

		for (var i = 0; i < blockSetListA.Count; i++)
		{
			var blockA = blockSetListA[i];
			var blockB = blockSetListB[i];
			if (!blockA.Equals(blockB))
			{
				return blockA.CompareTo(blockB);
			}
		}

		return 0;
	}
}