using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Melds;
using Koffing.Waits;

namespace Koffing.Calculations;

/// <summary>
/// Functions for calculating the readiness of a hand (a <see cref="BlockSet"/>).
/// </summary>
/// <remarks>
/// <para>
/// A hand being "ready" means that there is one tile remaining to make a valid complete hand of 14 tiles. This means
/// either four <see cref="Meld"/>s of 3 tiles each and a <see cref="Pair"/> (standard readiness) or one of the special
/// hand patterns of seven pairs/thirteen orphans. Note that a hand may be valid and complete but lack yaku, and thus
/// not be a hand that a player can go out on.
/// </para>
/// <para>
/// For ease of calculation, all the readiness functions expect a <see cref="BlockSet"/>, so that it can use the
/// semantic knowledge contained in the derived classes of <see cref="Block"/> (like <see cref="Meld"/> and
/// <see cref="Wait"/>). Given an <see cref="IEnumerable{TileSet.Tile}"/>, use
/// <see cref="Block.GetPossibleBlockSets(IEnumerable{Tiles.Tile})"/> to then feed the possible BlockSets into these
/// functions.
/// </para>
/// </remarks>
// TODO: We will see if it's helpful, but it may be worth adding a function here that does the work of taking in a
// collection of tiles and running it through these BlockSet functions.
public static class Readiness
{
	/// <summary>
	/// The type of readiness algorithm that was used for calculation.
	/// </summary>
	/// <remarks>
	/// This type is returned as an output parameter by several of the <see cref="Readiness"/> functions that will
	/// choose the correct readiness calculation algorithm based on the <see cref="Block"/>s of a <see cref="BlockSet"/>
	/// to indicate which algorithm was used.
	/// </remarks>
	public enum Type
	{
		Standard,
		SevenPairs,
		ThirteenOrphans,
	}

	/// <summary>
	/// Returns whether the current hand is ready, and the readiness algorithm used for calculation.
	/// </summary>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <param name="chosenReadinessType">
	/// Will contain a <see cref="Type"/> indicating the readiness algorithm used after returning.
	/// </param>
	/// <returns><see langword="true"/>, if the hand is ready, <see langword="false"/> otherwise.</returns>
	public static bool IsReady(this BlockSet hand, out Type chosenReadinessType)
	{
		return TilesToReady(hand, out chosenReadinessType) == 0;
	}

	/// <summary>
	/// Returns whether the current hand is past ready, that is, having a hand with more than enough tiles to be ready.
	/// It also returns the readiness algorithm used for calculation.
	/// </summary>
	/// <remarks>
	/// Note that this may or may not be the same as a "valid complete" hand. This function does not consider if all
	/// <see cref="Block"/>s in the <see cref="BlockSet"/> are not a <see cref="Wait"/> or <see cref="Orphan"/>, nor
	/// that there are only 14 tiles.
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <param name="chosenReadinessType">
	/// Will contain a <see cref="Type"/> indicating the readiness algorithm used after returning.
	/// </param>
	/// <returns><see langword="true"/>, if the hand is ready, <see langword="false"/> otherwise.</returns>
	public static bool IsPastReady(this BlockSet hand, out Type chosenReadinessType)
	{
		return TilesToReady(hand, out chosenReadinessType) < 0;
	}

	/// <summary>
	/// Returns the number of tiles that the hand must take in to be ready, as well as the readiness algorithm used for
	/// calculation.
	/// </summary>
	/// <remarks>
	/// Note that in certain cases, non-positive numbers like zero and below are valid results, representing a
	/// "past-ready" hand (see <see cref="IsPastReady(BlockSet, out Type)"/>).
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <param name="chosenReadinessType">
	/// Will contain a <see cref="Type"/> indicating the readiness algorithm used after returning.
	/// </param>
	/// <returns>The number of tiles remaining until a ready hand.</returns>
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

	/// <summary>
	/// Returns the number of tiles that the hand must take in to be ready, using the specified readiness algorithm
	/// calculation.
	/// </summary>
	/// <remarks>
	/// Note that in certain cases, non-positive numbers like zero and below are valid results, representing a
	/// "past-ready" hand (see <see cref="IsPastReady(BlockSet, out Type)"/>).
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <param name="readinessType">
	/// A <see cref="Type"/> indicating the readiness algorithm to use for calculation.
	/// </param>
	/// <returns>The number of tiles remaining until a ready hand based on the specified algorithm.</returns>
	/// <exception cref="InvalidOperationException">Thrown when given an unknown readiness type.</exception>
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

	/// <summary>
	/// Returns the number of tiles that the hand must take in to be ready, using the standard readiness calculation
	/// algorithm, which assumes the valid complete hand will be made up of four <see cref="Meld"/>s of 3 tiles, and a
	/// <see cref="Pair"/>.
	/// </summary>
	/// <remarks>
	/// Note that in certain cases, non-positive numbers like zero and below are valid results, representing a
	/// "past-ready" hand (see <see cref="IsPastReady(BlockSet, out Type)"/>).
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <returns>The number of tiles remaining until a ready hand based on the standard algorithm.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when given a <see cref="BlockSet"/> that contains a <see cref="SevenPairsWait"/> or
	/// <see cref="ThirteenOrphansWait"/>, as these hands cannot be correctly calculated using the standard algorithm.
	/// </exception>
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

	/// <summary>
	/// Returns the number of tiles that the hand must take in to be ready, using the seven pairs readiness calculation
	/// algorithm, which assumes the valid complete hand will be made up of seven <see cref="Pair"/>s.
	/// </summary>
	/// <remarks>
	/// Note that in certain cases, non-positive numbers like zero and below are valid results, representing a
	/// "past-ready" hand (see <see cref="IsPastReady(BlockSet, out Type)"/>).
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <returns>The number of tiles remaining until a ready hand based on the seven pairs algorithm.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when given a <see cref="BlockSet"/> that does NOT contain only a single <see cref="SevenPairsWait"/>,
	/// as such hands cannot be correctly calculated using the seven pairs algorithm. Note that this includes hands that
	/// have multiple <see cref="SevenPairsWait"/>s.
	/// </exception>
	public static int SevenPairsTilesToReady(BlockSet hand)
	{
		var sevenPairsWait = hand.OfType<SevenPairsWait>().SingleOrDefault();
		if (sevenPairsWait == default)
		{
			throw new InvalidOperationException($"Passed in hand without a lone single seven pairs wait when calculating SevenPairsTilesToReady: {hand}");
		}
		return 6 - sevenPairsWait.Pairs.Count();
	}

	/// <summary>
	/// Returns the number of tiles that the hand must take in to be ready, using the thirteen orphans readiness
	/// calculation algorithm, which assumes the valid complete hand will be made up of twelve distinct terminal and
	/// Zi suit <see cref="Orphan"/>s, and the last remaining distinct terminal/Zi suit in a <see cref="Pair"/>.
	/// </summary>
	/// <remarks>
	/// Note that in certain cases, non-positive numbers like zero and below are valid results, representing a
	/// "past-ready" hand (see <see cref="IsPastReady(BlockSet, out Type)"/>).
	/// </remarks>
	/// <param name="hand">The hand to analyze as a <see cref="BlockSet"/>.</param>
	/// <returns>The number of tiles remaining until a ready hand based on the thirteen orphans algorithm.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when given a <see cref="BlockSet"/> that does NOT contain only a single
	/// <see cref="ThirteenOrphansWait"/>, as such hands cannot be correctly calculated using the thirteen orphans
	/// algorithm. Note that this includes hands that have multiple <see cref="ThirteenOrphansWait"/>s.
	/// </exception>
	public static int ThirteenOrphansTilesToReady(BlockSet hand)
	{
		var thirteenOrphansWait = hand.OfType<ThirteenOrphansWait>().SingleOrDefault();
		if (thirteenOrphansWait == default)
		{
			throw new InvalidOperationException($"Passed in hand without a lone single thirteen orphans wait when calculating ThirteenOrphansTilesToReady: {hand}");
		}
		var distinctTileCount = thirteenOrphansWait.Distinct().Count();
		var pairsCount = Math.Min(thirteenOrphansWait.Pairs.Count(), 1);
		return 13 - distinctTileCount - pairsCount;
	}
}