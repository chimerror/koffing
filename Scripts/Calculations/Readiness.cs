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

	public static bool IsReady(this BlockSet hand, out Type chosenReadinessType)
	{
		return TilesToReady(hand, out chosenReadinessType) == 0;
	}

	// Note that this is not _complete_ because we're not checking if there are any waits or orphans, nor if it has more
	// tiles than possible.
	public static bool IsPastReady(this BlockSet hand, out Type chosenReadinessType)
	{
		return TilesToReady(hand, out chosenReadinessType) < 0;
	}

	public static int TilesToReady(BlockSet hand, out Type chosenReadinessType)
	{
		// UNTESTED: Looking around I did not notice any mocking libraries that both supported static methods and
		// weren't AI slop. I could write a test that runs through this code, but it would not truly verify the code's
		// functionality in picking the right function to call. So, alas.
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
		// UNTESTED: Looking around I did not notice any mocking libraries that both supported static methods and
		// weren't AI slop. I could write a test that runs through this code, but it would not truly verify the code's
		// functionality in picking the right function to call. So, alas.
		return readinessType switch
		{
			Type.Standard => StandardTilesToReady(hand),
			Type.SevenPairs => SevenPairsTilesToReady(hand),
			Type.ThirteenOrphans => ThirteenOrphansTilesToReady(hand),
			_ => throw new InvalidOperationException($"Passed in unknown readiness type when calculating TilesToReady!: {readinessType}"),
		};
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
		var sevenPairsWait = hand.OfType<SevenPairsWait>().SingleOrDefault();
		if (sevenPairsWait == default)
		{
			throw new InvalidOperationException($"Passed in hand without a seven pairs wait when calculating SevenPairsTilesToReady: {hand}");
		}
		return 6 - sevenPairsWait.Pairs.Count();
	}

	public static int ThirteenOrphansTilesToReady(BlockSet hand)
	{
		var thirteenOrphansWait = hand.OfType<ThirteenOrphansWait>().SingleOrDefault();
		if (thirteenOrphansWait == default)
		{
			throw new InvalidOperationException($"Passed in hand without a thirteen orphans wait when calculating ThirteenOrphansTilesToReady: {hand}");
		}
		var distinctTileCount = thirteenOrphansWait.Distinct().Count();
		var pairsCount = Math.Min(thirteenOrphansWait.Pairs.Count(), 1);
		return 13 - distinctTileCount - pairsCount;
	}
}