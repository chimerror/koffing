using System;
using System.Linq;

public static class Readiness
{
	public enum Type
	{
		Standard,
		SevenPairs,
		ThirteenOrphans,
	}

	public static int TilesToReady(BlockSet hand, out Type chosenReadinessType)
	{
		// TODOTODO: Test
		if (hand.Any(b => b is SevenPairsWait))
		{
			chosenReadinessType = Type.SevenPairs;
			return SevenPairsTilesToReady(hand);
		}
		else if (hand.Any(b => b is ThirteenOrphansWait))
		{
			chosenReadinessType = Type.ThirteenOrphans;
			return ThirteenOrphansTilesToReady(hand);
		}
		else
		{
			chosenReadinessType = Type.Standard;
			return StandardTilesToReady(hand);
		}
	}

	public static int TilesToReady(BlockSet hand, Type readinessType)
	{
		// TODOTODO: Test
		switch (readinessType)
		{
			case Type.Standard:
				return StandardTilesToReady(hand);

			case Type.SevenPairs:
				return SevenPairsTilesToReady(hand);

			case Type.ThirteenOrphans:
				return ThirteenOrphansTilesToReady(hand);

			default:
				throw new InvalidOperationException($"Passed in unknown readiness type when calculating TilesToReady!: {readinessType}");
		}
	}

	public static int StandardTilesToReady(BlockSet hand)
	{
		var currentCount = 8;
		var blocksCounted = 0;
		var pairFound = false;
		foreach (var block in hand)
		{
			if (block is SevenPairsWait || block is ThirteenOrphansWait)
			{
				throw new InvalidOperationException($"Passed in special hand when calculating StandardTilesToReady, which can not produce the correct calculation: {block}");
			}
			else if (blocksCounted < 4 && block is Meld && block is not Pair)
			{
				currentCount -= 2;
				blocksCounted++;
			}
			else if (!pairFound && (block is PairWait || block is Pair))
			{
				if (blocksCounted < 4)
				{
					currentCount--;
					blocksCounted++;
				}
				pairFound = true;
			}
			else if (blocksCounted < 4 && block is Wait && block is not Orphan)
			{
				currentCount--;
				blocksCounted++;
			}
		}

		if (blocksCounted == 4 && pairFound)
		{
			currentCount--;
		}
		return currentCount;
	}

	public static int SevenPairsTilesToReady(BlockSet hand)
	{
		// TODOTODO: Test
		var sevenPairsWait = hand.OfType<SevenPairsWait>().Single();
		return 6 - sevenPairsWait.Pairs.Count();
	}

	public static int ThirteenOrphansTilesToReady(BlockSet hand)
	{
		// TODOTODO: Test
		var thirteenOrphansWait = hand.OfType<ThirteenOrphansWait>().Single();
		var distinctTileCount = thirteenOrphansWait.Distinct().Count();
		var pairsCount = Math.Min(thirteenOrphansWait.Pairs.Count(), 1);
		return 13 - distinctTileCount - pairsCount;
	}
}