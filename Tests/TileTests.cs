using System;
using System.Collections.Generic;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class TileTests
{
	[TestCase]
	public static void RawRanksAreCorrect()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!IsValidTile(suit, rank))
				{
					continue;
				}

				var tile = new Tile(suit, rank);
				var expectedRawRank = rank == 0 ? 5 : rank;
				AssertThat(tile.RawRank).IsEqual(expectedRawRank);
			}
		}
	}

	[TestCase]
	public static void RawEqualsIsCorrectPositiveCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!IsValidTile(suit, rank))
				{
					continue;
				}

				var tileA = new Tile(suit, rank);
				var tileB = new Tile(suit, rank);

				AssertThat(tileA.RawEquals(tileB)).IsTrue(); // tile A should rawly equal identical tile B
				AssertThat(tileB.RawEquals(tileA)).IsTrue(); // tile B should rawly equal identical tile A

				if (rank == 0)
				{
					var tileC = new Tile(suit, 5);
					AssertThat(tileA.RawEquals(tileC)).IsTrue(); // red five A should rawly equal non-red five C
					AssertThat(tileC.RawEquals(tileA)).IsTrue(); // non-red five C should rawly equal red five A
				}
			}
		}
	}

	[TestCase]
	public static void RawEqualsIsCorrectNegativeCases()
	{
		foreach (var suitA in Enum.GetValues<Suit>())
		{
			for (var rankA = 0; rankA <= 9; rankA++)
			{
				if (!IsValidTile(suitA, rankA))
				{
					continue;
				}

				var tileA = new Tile(suitA, rankA);
				foreach (var suitB in Enum.GetValues<Suit>())
				{
					if (suitA == suitB || !IsValidTile(suitB, rankA))
					{
						continue;
					}

					var tileB = new Tile(suitB, rankA);

					// tile A should not rawly equal tile B with a different suit but the same rank
					AssertThat(tileA.RawEquals(tileB)).IsFalse();

					// tile B should not rawly equal tile A with a different suit but the same rank
					AssertThat(tileB.RawEquals(tileA)).IsFalse();
				}

				for (var rankC = 0; rankC <= 9; rankC++)
				{
					if (rankA == rankC ||
						(rankA == 5 && rankC == 0) ||
						(rankA == 0 && rankC == 5)||
						!IsValidTile(suitA, rankC))
					{
						continue;
					}

					var tileC = new Tile(suitA, rankC);

					// tile A should not rawly equal tile C with a different rank but the same suit
					AssertThat(tileA.RawEquals(tileC)).IsFalse();

					// tile C should not rawly equal tile A with a different rank but the same suit
					AssertThat(tileC.RawEquals(tileA)).IsFalse();
				}
			}
		}
	}

	[TestCase]
	[DataPoint(nameof(CompareTileTestCases))]
	public static void CompareTilesIsCorrect(Tile tileA, Tile tileB, int expectedComparisonValue)
	{
		AssertThat(tileA.CompareTo(tileB)).IsEqual(expectedComparisonValue);
	}

	private static bool IsValidTile(Suit suit, int rank)
	{
		return suit != Suit.Zi ? (rank >= 0 && rank <= 9) : (rank >= 1 && rank <= 7);
	}

	private static IEnumerable<object[]> CompareTileTestCases()
	{
		yield return [new Tile(), null, 1]; // comparing to null tile
		yield return [new Tile(Suit.Man), new Tile(Suit.Pin), -1]; // comparing man to pin
		yield return [new Tile(Suit.Pin), new Tile(Suit.Man), 1]; // comparing pin to man
		yield return [new Tile(Suit.Pin), new Tile(Suit.Sou), -1]; // comparing pin to sou
		yield return [new Tile(Suit.Sou), new Tile(Suit.Pin), 1]; // comparing sou to pin
		yield return [new Tile(Suit.Sou), new Tile(Suit.Zi), -1]; // comparing sou to zi
		yield return [new Tile(Suit.Zi), new Tile(Suit.Sou), 1]; // comparing zi to sou
		yield return [new Tile(Suit.Man, 5), new Tile(Suit.Man, 5), 0]; // comparing identical non-red 5s
		yield return [new Tile(Suit.Man, 0), new Tile(Suit.Man, 0), 0]; // comparing identical red 5s
		yield return [new Tile(Suit.Man, 0), new Tile(Suit.Man, 5), 1]; // comparing red 5 to non-red 5
		yield return [new Tile(Suit.Man, 5), new Tile(Suit.Man, 0), -1]; // comparing non-red 5 to red 5
		yield return [new Tile(Suit.Man, 0), new Tile(Suit.Man, 2), 1]; // comparing red 5 to 2
		yield return [new Tile(Suit.Man, 2), new Tile(Suit.Man, 0), -1]; // comparing 2 to red 5
		yield return [new Tile(Suit.Man, 0), new Tile(Suit.Man, 7), -1]; // comparing red 5 to 7
		yield return [new Tile(Suit.Man, 7), new Tile(Suit.Man, 0), 1]; // comparing 7 to red 5
		yield return [new Tile(Suit.Man, 2), new Tile(Suit.Man, 7), -1]; // comparing 2 to 7
		yield return [new Tile(Suit.Man, 7), new Tile(Suit.Man, 2), 1]; // comparing 7 to 2
	}
}