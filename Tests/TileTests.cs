using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
[RequireGodotRuntime]
public class TileTests
{
	[Before]
	public static void Setup()
	{
		SetupLogging();
	}

	[After]
	public static void TearDown()
	{
		TearDownLogging();
	}

	[TestCase]
	public static void RawRanksAreCorrect()
	{
		LoggingPrefix = nameof(RawRanksAreCorrect);
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!IsValidTile(suit, rank))
				{
					continue;
				}

				var tile = AutoFree(new Tile(suit, rank));
				var expectedRawRank = rank == 0 ? 5 : rank;
				PrefixInfo($"Checking that raw rank of {rank} {suit} is {expectedRawRank}...");
				AssertThat(tile.RawRank).IsEqual(expectedRawRank);
			}
		}
	}

	[TestCase]
	public static void EqualsIsCorrectPositiveCases()
	{
		LoggingPrefix = nameof(EqualsIsCorrectPositiveCases);
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!IsValidTile(suit, rank))
				{
					continue;
				}

				var tileA = AutoFree(new Tile(suit, rank));
				var tileB = AutoFree(new Tile(suit, rank));

				PrefixInfo($"Checking that tile A {tileA} should equal identical tile B {tileB}");
				AssertThat(tileA.Equals(tileB)).IsTrue();
				PrefixInfo($"Checking that tile B {tileB} should equal identical tile A {tileB}");
				AssertThat(tileB.Equals(tileA)).IsTrue();

				tileB.FaceUp = false;
				PrefixInfo($"Checking that face-up tile A {tileA} should equal identical face-down tile B {tileB}");
				AssertThat(tileA.Equals(tileB)).IsTrue();
				PrefixInfo($"Checking that face-down tile B {tileB} should equal identical face-up tile A {tileB}");
				AssertThat(tileB.Equals(tileA)).IsTrue();
			}
		}
	}

	[TestCase]
	public static void EqualsIsCorrectNegativeCases()
	{
		LoggingPrefix = nameof(EqualsIsCorrectNegativeCases);
		foreach (var suitA in Enum.GetValues<Suit>())
		{
			for (var rankA = 0; rankA <= 9; rankA++)
			{
				if (!IsValidTile(suitA, rankA))
				{
					continue;
				}

				var tileA = AutoFree(new Tile(suitA, rankA));
				foreach (var suitB in Enum.GetValues<Suit>())
				{
					if (suitA == suitB || !IsValidTile(suitB, rankA))
					{
						continue;
					}

					var tileB = AutoFree(new Tile(suitB, rankA));

					PrefixInfo($"Checking that tile A {tileA} should NOT equal different-suited same-rank B {tileB}");
					AssertThat(tileA.Equals(tileB)).IsFalse();

					PrefixInfo($"Checking that tile B {tileB} should NOT equal different-suited same-rank A {tileA}");
					AssertThat(tileB.Equals(tileA)).IsFalse();
				}

				for (var rankC = 0; rankC <= 9; rankC++)
				{
					if (rankA == rankC ||
						!IsValidTile(suitA, rankC))
					{
						continue;
					}

					var tileC = AutoFree(new Tile(suitA, rankC));

					PrefixInfo($"Checking that tile A {tileA} should NOT equal different-rank same-suit C {tileC}");
					AssertThat(tileA.Equals(tileC)).IsFalse();

					PrefixInfo($"Checking that tile C {tileC} should NOT equal different-rank same-suit A {tileA}");
					AssertThat(tileC.Equals(tileA)).IsFalse();
				}
			}
		}
	}

	[TestCase]
	public static void RawEqualsIsCorrectPositiveCases()
	{
		LoggingPrefix = nameof(RawEqualsIsCorrectPositiveCases);
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!IsValidTile(suit, rank))
				{
					continue;
				}

				var tileA = AutoFree(new Tile(suit, rank));
				var tileB = AutoFree(new Tile(suit, rank));

				PrefixInfo($"Checking that tile A {tileA} should rawly equal identical tile B {tileB}");
				AssertThat(tileA.RawEquals(tileB)).IsTrue();
				PrefixInfo($"Checking that tile B {tileB} should rawly equal identical tile A {tileB}");
				AssertThat(tileB.RawEquals(tileA)).IsTrue();

				tileB.FaceUp = false;
				PrefixInfo($"Checking that face-up tile A {tileA} should rawly equal identical face-down tile B {tileB}");
				AssertThat(tileA.RawEquals(tileB)).IsTrue();
				PrefixInfo($"Checking that face-down tile B {tileB} should rawly equal identical face-up tile A {tileB}");
				AssertThat(tileB.RawEquals(tileA)).IsTrue();

				if (rank == 0)
				{
					var tileC = AutoFree(new Tile(suit, 5));
					PrefixInfo($"Checking that red five A {tileA} should rawly equal non-red five C {tileC}");
					AssertThat(tileA.RawEquals(tileC)).IsTrue();
					PrefixInfo($"Checking that non-red five C {tileC} should rawly equal red five A {tileA}");
					AssertThat(tileC.RawEquals(tileA)).IsTrue();
				}
			}
		}
	}

	[TestCase]
	public static void RawEqualsIsCorrectNegativeCases()
	{
		LoggingPrefix = nameof(RawEqualsIsCorrectNegativeCases);
		foreach (var suitA in Enum.GetValues<Suit>())
		{
			for (var rankA = 0; rankA <= 9; rankA++)
			{
				if (!IsValidTile(suitA, rankA))
				{
					continue;
				}

				var tileA = AutoFree(new Tile(suitA, rankA));
				foreach (var suitB in Enum.GetValues<Suit>())
				{
					if (suitA == suitB || !IsValidTile(suitB, rankA))
					{
						continue;
					}

					var tileB = AutoFree(new Tile(suitB, rankA));

					PrefixInfo($"Checking that tile A {tileA} should NOT rawly equal different-suited same-rank B {tileB}");
					AssertThat(tileA.RawEquals(tileB)).IsFalse();

					PrefixInfo($"Checking that tile B {tileB} should NOT rawly equal different-suited same-rank A {tileA}");
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

					var tileC = AutoFree(new Tile(suitA, rankC));

					PrefixInfo($"Checking that tile A {tileA} should NOT rawly equal different-rank same-suit C {tileC}");
					AssertThat(tileA.RawEquals(tileC)).IsFalse();

					PrefixInfo($"Checking that tile C {tileC} should NOT rawly equal different-rank same-suit A {tileA}");
					AssertThat(tileC.RawEquals(tileA)).IsFalse();
				}
			}
		}
	}

	[TestCase]
	[DataPoint(nameof(EqualsNegativeEdgeCases))]
	public static void EqualsIsCorrectNegativeEdgeCases(Tile tileA, object objectB, string because)
	{
		PrefixInfo($"Checking that tile A {tileA} should not equal object B {objectB} when {because}");
		AssertThat(tileA.Equals(objectB)).IsFalse();

		if (objectB == null)
		{
			Tile tileB = null;
			PrefixInfo($"Checking that tile A {tileA} should not equal null tile B {tileB}");
			AssertThat(tileA.Equals(tileB)).IsFalse();
		}
	}

	[TestCase]
	[DataPoint(nameof(CompareTileTestCases))]
	public static void CompareTilesIsCorrect(Tile tileA, Tile tileB, int expectedComparisonValue, string because)
	{
		LoggingPrefix = nameof(CompareTilesIsCorrect);
		PrefixInfo($"Checking that Tile.CompareTo is {expectedComparisonValue} when {because}");
		AssertThat(tileA.CompareTo(tileB)).IsEqual(expectedComparisonValue);
	}

	[TestCase]
	[DataPoint(nameof(TileNotationTestCases))]
	public static void ToTilesAndNotationFromTilesAreCorrect(string notation, List<Tile> tiles, string because)
	{
		LoggingPrefix = nameof(ToTilesAndNotationFromTilesAreCorrect);

		var actualToTilesOutput = notation.ToTiles(true).ToList();
		PrefixInfo($"Checking that calling ToTiles on notation \"{notation}\"  produces correct tiles when {because}");
		AssertArray(actualToTilesOutput).ContainsExactly(tiles);

		var actualNotationFromTilesOutput = tiles.NotationFromTiles();
		PrefixInfo($"Checking that NotationFromTiles produces matching notation \"{notation}\" when {because}");
		AssertThat(actualNotationFromTilesOutput).IsEqual(notation);
	}

	[TestCase]
	[DataPoint(nameof(GetTileHashCodeTestCases))]
	public static void GetTileHashCodeIsCorrect(Tile tile, int expectedHashCode)
	{
		LoggingPrefix = nameof(GetTileHashCodeIsCorrect);

		PrefixInfo($"Checking that GetHashCode for {tile} is {expectedHashCode}");
		AssertThat(tile.GetHashCode()).IsEqual(expectedHashCode);
	}

	private static bool IsValidTile(Suit suit, int rank)
	{
		return suit != Suit.Zi ? (rank >= 0 && rank <= 9) : (rank >= 1 && rank <= 7);
	}


	private static IEnumerable<object[]> EqualsNegativeEdgeCases()
	{
		yield return ["4m".ToAutoFreeTile(), null, "comparing to null"];
		yield return ["4m".ToAutoFreeTile(), "4m", "comparing to a non-tile type"];
	}

	private static IEnumerable<object[]> CompareTileTestCases()
	{
		yield return [AutoFree(new Tile()), null, 1, "comparing to null tile"];
		yield return [AutoFree(new Tile(Suit.Man)), AutoFree(new Tile(Suit.Pin)), -1, "comparing man to pin"];
		yield return [AutoFree(new Tile(Suit.Pin)), AutoFree(new Tile(Suit.Man)), 1, "comparing pin to man"];
		yield return [AutoFree(new Tile(Suit.Pin)), AutoFree(new Tile(Suit.Sou)), -1, "comparing pin to sou"];
		yield return [AutoFree(new Tile(Suit.Sou)), AutoFree(new Tile(Suit.Pin)), 1, "comparing sou to pin"];
		yield return [AutoFree(new Tile(Suit.Sou)), AutoFree(new Tile(Suit.Zi)), -1, "comparing sou to zi"];
		yield return [AutoFree(new Tile(Suit.Zi)), AutoFree(new Tile(Suit.Sou)), 1, "comparing zi to sou"];
		yield return [
			AutoFree(new Tile(Suit.Man, 5)),
			AutoFree(new Tile(Suit.Man, 5)),
			0,
			"comparing identical non-red 5s"];
		yield return [
			AutoFree(new Tile(Suit.Man, 0)),
			AutoFree(new Tile(Suit.Man, 0)),
			0,
			"comparing identical red 5s"];
		yield return [
			AutoFree(new Tile(Suit.Man, 0)),
			AutoFree(new Tile(Suit.Man, 5)),
			1,
			"comparing red 5 to non-red 5"];
		yield return [
			AutoFree(new Tile(Suit.Man, 5)),
			AutoFree(new Tile(Suit.Man, 0)),
			-1,
			"comparing non-red 5 to red 5"];
		yield return [AutoFree(new Tile(Suit.Man, 0)), AutoFree(new Tile(Suit.Man, 2)), 1, "comparing red 5 to 2"];
		yield return [AutoFree(new Tile(Suit.Man, 2)), AutoFree(new Tile(Suit.Man, 0)), -1, "comparing 2 to red 5"];
		yield return [AutoFree(new Tile(Suit.Man, 0)), AutoFree(new Tile(Suit.Man, 7)), -1, "comparing red 5 to 7"];
		yield return [AutoFree(new Tile(Suit.Man, 7)), AutoFree(new Tile(Suit.Man, 0)), 1, "comparing 7 to red 5"];
		yield return [AutoFree(new Tile(Suit.Man, 2)), AutoFree(new Tile(Suit.Man, 7)), -1, "comparing 2 to 7"];
		yield return [AutoFree(new Tile(Suit.Man, 7)), AutoFree(new Tile(Suit.Man, 2)), 1, "comparing 7 to 2"];
	}

	private static IEnumerable<object[]> TileNotationTestCases()
	{
		yield return ["1p", new List<Tile> { AutoFree(new Tile(Suit.Pin, 1)) }, "provided single number tile"];
		yield return ["4z", new List<Tile> { AutoFree(new Tile(Suit.Zi, 4)) }, "provided single honor tile"];

		yield return [
			"123p",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Pin, 1)),
				AutoFree(new Tile(Suit.Pin, 2)),
				AutoFree(new Tile(Suit.Pin, 3)),
			},
			"provided multiple tiles in one suit",
		];

		yield return [
			"729p",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Pin, 7)),
				AutoFree(new Tile(Suit.Pin, 2)),
				AutoFree(new Tile(Suit.Pin, 9))
			},
			"keeping order of multiple out-of-order tiles in one suit",
		];

		yield return [
			"724z729p",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Zi, 7)),
				AutoFree(new Tile(Suit.Zi, 2)),
				AutoFree(new Tile(Suit.Zi, 4)),
				AutoFree(new Tile(Suit.Pin, 7)),
				AutoFree(new Tile(Suit.Pin, 2)),
				AutoFree(new Tile(Suit.Pin, 9)),
			},
			"keeping order of multiple out-of-order suits with out-of-order tiles",
		];

		yield return [
			"123p3z5m",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Pin, 1)),
				AutoFree(new Tile(Suit.Pin, 2)),
				AutoFree(new Tile(Suit.Pin, 3)),
				AutoFree(new Tile(Suit.Zi, 3)),
				AutoFree(new Tile(Suit.Man, 5)),
			},
			"provided multiple suits, some with single tiles",
		];

		yield return [
			"123p333z45m",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Pin, 1)),
				AutoFree(new Tile(Suit.Pin, 2)),
				AutoFree(new Tile(Suit.Pin, 3)),
				AutoFree(new Tile(Suit.Zi, 3)),
				AutoFree(new Tile(Suit.Zi, 3)),
				AutoFree(new Tile(Suit.Zi, 3)),
				AutoFree(new Tile(Suit.Man, 4)),
				AutoFree(new Tile(Suit.Man, 5)),
			},
			"provided multiple suits, all with multiple tiles",
		];

		yield return [
			"5055p",
			new List<Tile>
			{
				AutoFree(new Tile(Suit.Pin, 5)),
				AutoFree(new Tile(Suit.Pin, 0)),
				AutoFree(new Tile(Suit.Pin, 5)),
				AutoFree(new Tile(Suit.Pin, 5)),
			},
			"provided kong of 5s (one red 5)",
		];
	}

	private static IEnumerable<object[]> GetTileHashCodeTestCases()
	{
		yield return ["0m".ToAutoFreeTile(), 2];
		yield return ["1m".ToAutoFreeTile(), 4];
		yield return ["2m".ToAutoFreeTile(), 8];
		yield return ["3m".ToAutoFreeTile(), 16];
		yield return ["4m".ToAutoFreeTile(), 32];
		yield return ["5m".ToAutoFreeTile(), 64];
		yield return ["6m".ToAutoFreeTile(), 128];
		yield return ["7m".ToAutoFreeTile(), 256];
		yield return ["8m".ToAutoFreeTile(), 512];
		yield return ["9m".ToAutoFreeTile(), 1024];
		yield return ["0p".ToAutoFreeTile(), 3];
		yield return ["1p".ToAutoFreeTile(), 9];
		yield return ["2p".ToAutoFreeTile(), 27];
		yield return ["3p".ToAutoFreeTile(), 81];
		yield return ["4p".ToAutoFreeTile(), 243];
		yield return ["5p".ToAutoFreeTile(), 729];
		yield return ["6p".ToAutoFreeTile(), 2187];
		yield return ["7p".ToAutoFreeTile(), 6561];
		yield return ["8p".ToAutoFreeTile(), 19683];
		yield return ["9p".ToAutoFreeTile(), 59049];
		yield return ["0s".ToAutoFreeTile(), 5];
		yield return ["1s".ToAutoFreeTile(), 25];
		yield return ["2s".ToAutoFreeTile(), 125];
		yield return ["3s".ToAutoFreeTile(), 625];
		yield return ["4s".ToAutoFreeTile(), 3125];
		yield return ["5s".ToAutoFreeTile(), 15625];
		yield return ["6s".ToAutoFreeTile(), 78125];
		yield return ["7s".ToAutoFreeTile(), 390625];
		yield return ["8s".ToAutoFreeTile(), 1953125];
		yield return ["9s".ToAutoFreeTile(), 9765625];
		yield return ["1z".ToAutoFreeTile(), 49];
		yield return ["2z".ToAutoFreeTile(), 343];
		yield return ["3z".ToAutoFreeTile(), 2401];
		yield return ["4z".ToAutoFreeTile(), 16807];
		yield return ["5z".ToAutoFreeTile(), 117649];
		yield return ["6z".ToAutoFreeTile(), 823543];
		yield return ["7z".ToAutoFreeTile(), 5764801];
	}
}