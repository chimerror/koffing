using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class WaitTests
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
	[DataPoint(nameof(GetFirstLevelWaitsTestCases))]
	public static void GetFirstLevelWaitsIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(GetFirstLevelWaitsIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Wait.GetFirstLevelWaits(tiles);
		PrefixInfo($"Checking that Wait.GetFirstLevelWaits with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	public static void CanCreateOrphan()
	{
		LoggingPrefix = nameof(CanCreateOrphan);

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!Tile.IsValidTile(suit, rank))
				{
					continue;
				}

				var tile = new Tile(suit, rank);
				PrefixInfo($"Checking that we can create an orphan from tile \"{tile}\"...");
				var orphan = new Orphan(tile);
				AssertThat(tile).IsEqual(orphan.Tiles.Single());
			}
		}
	}

	[TestCase]
	[DataPoint(nameof(RyanmenGetPossibleForTileTestCases))]
	public static void RyanmenGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(RyanmenGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Ryanmen.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Ryanmen.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(RyanmenGetPossibleTestCases))]
	public static void RyanmenGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(RyanmenGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Ryanmen.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Ryanmen.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(KanchanGetPossibleForTileTestCases))]
	public static void KanchanGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(KanchanGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Kanchan.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Kanchan.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(KanchanGetPossibleTestCases))]
	public static void KanchanGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(KanchanGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Kanchan.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Kanchan.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(PenchanGetPossibleForTileTestCases))]
	public static void PenchanGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(PenchanGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Penchan.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Penchan.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(PenchanGetPossibleTestCases))]
	public static void PenchanGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(PenchanGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Penchan.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Penchan.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(SevenPairsWaitEqualsTestCases))]
	public static void SevenPairsWaitEqualsIsCorrect(SevenPairsWait blockA, SevenPairsWait blockB, bool expectedResult, string because)
	{
		LoggingPrefix = nameof(SevenPairsWaitEqualsIsCorrect);

		var outcomeString = expectedResult ? "does equal" : "does NOT equal";
		PrefixInfo($"Checking that seven pairs wait A \"{blockA}\" {outcomeString} seven pairs wait B \"{blockB}\" because {because}");
		AssertThat(expectedResult).IsEqual(blockA.Equals(blockB));
		if (blockB != null)
		{
			PrefixInfo($"Checking that seven pairs wait B \"{blockB}\" {outcomeString} seven pairs wait A \"{blockA}\" because {because}");
			AssertThat(expectedResult).IsEqual(blockB.Equals(blockA));
		}
	}

	[TestCase]
	[DataPoint(nameof(SevenPairsWaitGetPossibleForTileTestCases))]
	public static void SevenPairsWaitGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(SevenPairsWaitGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = SevenPairsWait.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that SevenPairsWait.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(SevenPairsWaitGetPossibleTestCases))]
	public static void SevenPairsWaitGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(SevenPairsWaitGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = SevenPairsWait.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that SevenPairsWait.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(ThirteenOrphansWaitEqualsTestCases))]
	public static void ThirteenOrphansWaitEqualsIsCorrect(ThirteenOrphansWait blockA, ThirteenOrphansWait blockB, bool expectedResult, string because)
	{
		LoggingPrefix = nameof(ThirteenOrphansWaitEqualsIsCorrect);

		var outcomeString = expectedResult ? "does equal" : "does NOT equal";
		PrefixInfo($"Checking that thirteen orphans wait A \"{blockA}\" {outcomeString} thirteen orphans wait B \"{blockB}\" because {because}");
		AssertThat(expectedResult).IsEqual(blockA.Equals(blockB));
		if (blockB != null)
		{
			PrefixInfo($"Checking that thirteen orphans wait B \"{blockB}\" {outcomeString} thirteen orphans wait A \"{blockA}\" because {because}");
			AssertThat(expectedResult).IsEqual(blockB.Equals(blockA));
		}
	}

	[TestCase]
	[DataPoint(nameof(ThirteenOrphansWaitGetPossibleTestCases))]
	public static void ThirteenOrphansWaitGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(ThirteenOrphansWaitGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = ThirteenOrphansWait.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that ThirteenOrphansWait.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	private static IEnumerable<object[]> GetFirstLevelWaitsTestCases()
	{
		// TODO: This is a thirteen orphans wait, we may have to change this test a bit when we add that type of wait.
		yield return ["19m19s19p1234567z", new List<MadeBlockContext>(), "there are no waits"];
		yield return
		[
			"11234467899s",
			new List<MadeBlockContext>()
			{
				new(new PairWait("11s".ToTiles()), "234467899s".ToTiles()),
				new(new PairWait("44s".ToTiles()), "112367899s".ToTiles()),
				new(new PairWait("99s".ToTiles()), "112344678s".ToTiles()),
				new(new Ryanmen("23s".ToTiles()), "114467899s".ToTiles()),
				new(new Ryanmen("34s".ToTiles()), "112467899s".ToTiles()),
				new(new Ryanmen("67s".ToTiles()), "112344899s".ToTiles()),
				new(new Ryanmen("78s".ToTiles()), "112344699s".ToTiles()),
				new(new Kanchan("13s".ToTiles()), "124467899s".ToTiles()),
				new(new Kanchan("24s".ToTiles()), "113467899s".ToTiles()),
				new(new Kanchan("46s".ToTiles()), "112347899s".ToTiles()),
				new(new Kanchan("68s".ToTiles()), "112344799s".ToTiles()),
				new(new Kanchan("79s".ToTiles()), "112344689s".ToTiles()),
				new(new Penchan("12s".ToTiles()), "134467899s".ToTiles()),
				new(new Penchan("89s".ToTiles()), "112344679s".ToTiles()),
			},
			"there are multiple waits of each type in one suit",
		];
		yield return
		[
			"1123m7899p",
			new List<MadeBlockContext>()
			{
				new(new PairWait("11m".ToTiles()), "23m7899p".ToTiles()),
				new(new PairWait("99p".ToTiles()), "1123m78p".ToTiles()),
				new(new Ryanmen("23m".ToTiles()), "11m7899p".ToTiles()),
				new(new Ryanmen("78p".ToTiles()), "1123m99p".ToTiles()),
				new(new Kanchan("13m".ToTiles()), "12m7899p".ToTiles()),
				new(new Kanchan("79p".ToTiles()), "1123m89p".ToTiles()),
				new(new Penchan("12m".ToTiles()), "13m7899p".ToTiles()),
				new(new Penchan("89p".ToTiles()), "1123m79p".ToTiles()),
			},
			"there are waits of each type in two suits",
		];
		yield return
		[
			"1177z",
			new List<MadeBlockContext>()
			{
				new(new PairWait("11z".ToTiles()), "77z".ToTiles()),
				new(new PairWait("77z".ToTiles()), "11z".ToTiles()),
			},
			"there are zi waits",
		];
	}

	private static IEnumerable<object[]> RyanmenGetPossibleForTileTestCases()
	{
		yield return ["5z", "46z", new List<MadeBlockContext>(), "passed in zi tiles"];
		yield return ["1m", "23m6z", new List<MadeBlockContext>(), "given a one"];
		yield return ["9m", "78m6z", new List<MadeBlockContext>(), "given a nine"];
		yield return ["5m", "78m6z", new List<MadeBlockContext>(), "given no neighbors"];
		yield return [
			"5m",
			"4m6z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("45m".ToTiles()), "6z".ToTiles()),
			},
			"only the lower neighbor is available",
		];
		yield return [
			"5p",
			"6p4z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("56p".ToTiles()), "4z".ToTiles()),
			},
			"only the upper neighbor is available",
		];
		yield return [
			"5s",
			"46s3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("45s".ToTiles()), "6s3z".ToTiles()),
				new(new Ryanmen("56s".ToTiles()), "4s3z".ToTiles()),
			},
			"given a middle non-red tile and both neighbors are available",
		];
		yield return [
			"2m",
			"13m6z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("23m".ToTiles()), "1m6z".ToTiles()),
			},
			"given a two and both neighbors",
		];
		yield return [
			"8m",
			"79m6z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("78m".ToTiles()), "9m6z".ToTiles()),
			},
			"given an eight and both neighbors",
		];
		yield return [
			"0m",
			"46m3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("40m".ToTiles()), "6m3z".ToTiles()),
				new(new Ryanmen("06m".ToTiles()), "4m3z".ToTiles()),
			},
			"given a red five and both neighbors are available",
		];
		yield return [
			"4m",
			"0m3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("40m".ToTiles()), "3z".ToTiles()),
			},
			"given a four and a red five is available",
		];
		yield return [
			"4m",
			"05m3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("40m".ToTiles()), "5m3z".ToTiles()),
				new(new Ryanmen("45m".ToTiles()), "0m3z".ToTiles()),
			},
			"given a four with a red five and a non-red five available",
		];
		yield return [
			"6m",
			"0m3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("06m".ToTiles()), "3z".ToTiles()),
			},
			"given a six and a red five is available",
		];
		yield return [
			"6m",
			"05m3z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("06m".ToTiles()), "5m3z".ToTiles()),
				new(new Ryanmen("56m".ToTiles()), "0m3z".ToTiles()),
			},
			"given a six with a red five and a non-red five available",
		];
	}

	private static IEnumerable<object[]> RyanmenGetPossibleTestCases()
	{
		yield return ["25p36s47m4z", new List<MadeBlockContext>(), "there are no neighboring tiles"];
		yield return
		[
			"2p3s34m5z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("34m".ToTiles()), "2p3s5z".ToTiles()),
			},
			"there is only one ryanmen",
		];
		yield return
		[
			"2p3s3478m5z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("34m".ToTiles()), "2p3s78m5z".ToTiles()),
				new(new Ryanmen("78m".ToTiles()), "2p3s34m5z".ToTiles()),
			},
			"there are two non-overlapping ryanmen in the same suit",
		];
		yield return
		[
			"2p3s3456m5z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("34m".ToTiles()), "2p3s56m5z".ToTiles()),
				new(new Ryanmen("45m".ToTiles()), "2p3s36m5z".ToTiles()),
				new(new Ryanmen("56m".ToTiles()), "2p3s34m5z".ToTiles()),
			},
			"there are multiple overlapping ryanmen in the same suit",
		];
		yield return
		[
			"2p334s56m5z",
			new List<MadeBlockContext>()
			{
				new(new Ryanmen("34s".ToTiles()), "2p3s56m5z".ToTiles()),
				new(new Ryanmen("56m".ToTiles()), "2p334s5z".ToTiles()),
			},
			"there are multiple ryanmen in the different suits",
		];
	}

	private static IEnumerable<object[]> KanchanGetPossibleForTileTestCases()
	{
		yield return ["5z", "37z", new List<MadeBlockContext>(), "passed in zi tiles"];
		yield return ["5m", "1240689m", new List<MadeBlockContext>(), "given no valid tiles"];
		yield return
		[
			"5m",
			"12340689m",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35m".ToTiles()), "1240689m".ToTiles()),
			},
			"given only the lower tile",
		];
		yield return
		[
			"5s",
			"12407689s",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("57s".ToTiles()), "1240689s".ToTiles()),
			},
			"given only the upper tile",
		];
		yield return
		[
			"5p",
			"123407689p",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35p".ToTiles()), "12407689p".ToTiles()),
				new(new Kanchan("57p".ToTiles()), "12340689p".ToTiles()),
			},
			"given both lower and upper tiles",
		];
		yield return
		[
			"2m",
			"12345m",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("24m".ToTiles()), "1235m".ToTiles()),
			},
			"given a two and the upper tile",
		];
		yield return
		[
			"8m",
			"56789m",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("68m".ToTiles()), "5789m".ToTiles()),
			},
			"given an eight and the lower tile",
		];
		yield return
		[
			"3s",
			"550s",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35s".ToTiles()), "50s".ToTiles()),
				new(new Kanchan("30s".ToTiles()), "55s".ToTiles()),
			},
			"given a three with both a red five and multiple non-red fives",
		];
		yield return
		[
			"7s",
			"550s",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("57s".ToTiles()), "50s".ToTiles()),
				new(new Kanchan("07s".ToTiles()), "55s".ToTiles()),
			},
			"given a seven with both a red five and multiple non-red fives",
		];
		yield return
		[
			"0m",
			"37m",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("30m".ToTiles()), "7m".ToTiles()),
				new(new Kanchan("07m".ToTiles()), "3m".ToTiles()),
			},
			"given a red five and both lower and upper tiles",
		];
	}

	private static IEnumerable<object[]> KanchanGetPossibleTestCases()
	{
		yield return ["25p36s47m4z", new List<MadeBlockContext>(), "there are no kanchan"];
		yield return
		[
			"2p3s35m5z",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35m".ToTiles()), "2p3s5z".ToTiles()),
			},
			"there is only one kanchan",
		];
		yield return
		[
			"24p35s3m5z",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35s".ToTiles()), "24p3m5z".ToTiles()),
				new(new Kanchan("24p".ToTiles()), "35s3m5z".ToTiles()),
			},
			"there are two kanchan in different suits",
		];
		yield return
		[
			"2p3s2345m5z",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("35m".ToTiles()), "2p3s24m5z".ToTiles()),
				new(new Kanchan("24m".ToTiles()), "2p3s35m5z".ToTiles()),
			},
			"there are two disjoint kanchan in the same suit",
		];
		yield return
		[
			"12345679p3s3m5z",
			new List<MadeBlockContext>()
			{
				new(new Kanchan("13p".ToTiles()), "245679p3s3m5z".ToTiles()),
				new(new Kanchan("24p".ToTiles()), "135679p3s3m5z".ToTiles()),
				new(new Kanchan("35p".ToTiles()), "124679p3s3m5z".ToTiles()),
				new(new Kanchan("46p".ToTiles()), "123579p3s3m5z".ToTiles()),
				new(new Kanchan("57p".ToTiles()), "123469p3s3m5z".ToTiles()),
				new(new Kanchan("79p".ToTiles()), "123456p3s3m5z".ToTiles()),
			},
			"there are multiple kanchan in the same suit",
		];
	}

	private static IEnumerable<object[]> PenchanGetPossibleForTileTestCases()
	{
		yield return ["2z", "13z", new List<MadeBlockContext>(), "passed in zi tiles"];
		yield return ["5s", "46s", new List<MadeBlockContext>(), "passed in middle tile"];
		yield return ["1m", "134506789m", new List<MadeBlockContext>(), "given no valid tiles for a one"];
		yield return ["2p", "234506789p", new List<MadeBlockContext>(), "given no valid tiles for a two"];
		yield return ["8m", "123450678m", new List<MadeBlockContext>(), "given no valid tiles for an eight"];
		yield return ["9s", "123450679s", new List<MadeBlockContext>(), "given no valid tiles for a nine"];
		yield return
		[
			"1m",
			"12234506789m",
			new List<MadeBlockContext>()
			{
				new(new Penchan("12m".ToTiles()), "1234506789m".ToTiles()),
			},
			"given multiple twos for a one",
		];
		yield return
		[
			"2p",
			"11234506789p",
			new List<MadeBlockContext>()
			{
				new(new Penchan("12p".ToTiles()), "1234506789p".ToTiles()),
			},
			"given multiple ones for a two",
		];
		yield return
		[
			"8s",
			"12345067899s",
			new List<MadeBlockContext>()
			{
				new(new Penchan("89s".ToTiles()), "1234506789s".ToTiles()),
			},
			"given multiple nines for an eight",
		];
		yield return
		[
			"9m",
			"12345067889m",
			new List<MadeBlockContext>()
			{
				new(new Penchan("89m".ToTiles()), "1234506789m".ToTiles()),
			},
			"given multiple eights for a nine",
		];
	}

	private static IEnumerable<object[]> PenchanGetPossibleTestCases()
	{
		yield return ["25p36s47m4z", new List<MadeBlockContext>(), "there are no kanchan"];
		yield return
		[
			"2p3s89m5z",
			new List<MadeBlockContext>()
			{
				new(new Penchan("89m".ToTiles()), "2p3s5z".ToTiles()),
			},
			"there is only one penchan",
		];
		yield return
		[
			"12p3s89m5z",
			new List<MadeBlockContext>()
			{
				new(new Penchan("89m".ToTiles()), "12p3s5z".ToTiles()),
				new(new Penchan("12p".ToTiles()), "89m3s5z".ToTiles()),
			},
			"there are two penchan in different suits",
		];
		yield return
		[
			"1p3s1289m5z",
			new List<MadeBlockContext>()
			{
				new(new Penchan("89m".ToTiles()), "1p3s12m5z".ToTiles()),
				new(new Penchan("12m".ToTiles()), "1p89m3s5z".ToTiles()),
			},
			"there are two penchan in the same suit",
		];
	}

	private static IEnumerable<object[]> SevenPairsWaitEqualsTestCases()
	{
		yield return
		[
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
				]
			),
			null,
			false,
			"null should not equal",
		];
		yield return
		[
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
				]
			),
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
				]
			),
			true,
			"they have the same tiles and pairs",
		];
		yield return
		[
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
				]
			),
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("24p".ToTiles()),
					new Pair("24p".ToTiles()),
				]
			),
			false,
			"pairs created should matter",
		];
		yield return
		[
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
				]
			),
			new SevenPairsWait(
				"2244p".ToTiles(),
				[
					new Pair("22p".ToTiles()),
					new Pair("44p".ToTiles()),
					new Pair("66p".ToTiles()),
				]
			),
			false,
			"count of pairs should matter",
		];
	}

	private static IEnumerable<object[]> SevenPairsWaitGetPossibleTestCases()
	{
		yield return ["25p36s47m4z", new List<MadeBlockContext>(), "there are no pairs"];
		yield return
		[
			"25p3647m44z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait("44z".ToTiles(), [new Pair("44z".ToTiles())]), "25p3647m".ToTiles()),
			},
			"there is a single pair",
		];
		yield return
		[
			"11223344z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]),
					[]),
			},
			"there are only pairs in a single suit",
		];
		yield return
		[
			"119m228s337p447z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"11m22s33p44z".ToTiles(),
					[
						new Pair("11m".ToTiles()),
						new Pair("22s".ToTiles()),
						new Pair("33p".ToTiles()),
						new Pair("44z".ToTiles()),
					]),
					"9m8s7p7z".ToTiles()),
			},
			"there are multiple pairs across all suits",
		];
		yield return
		[
			"5550m67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"5550m".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50m".ToTiles()),
					]),
					"67z".ToTiles()),
			},
			"there are four number suit fives",
		];
		yield return
		[
			"555s67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55s".ToTiles(),
					[
						new Pair("55s".ToTiles()),
					]),
					"5s67z".ToTiles()),
			},
			"there are three number suit fives, but no red fives",
		];
		yield return
		[
			"550p67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55p".ToTiles(),
					[
						new Pair("55p".ToTiles()),
					]),
					"0p67z".ToTiles()),
				new(new SevenPairsWait(
					"50p".ToTiles(),
					[
						new Pair("50p".ToTiles()),
					]),
					"5p67z".ToTiles()),
			},
			"there are three number suit fives with one red five",
		];
		yield return
		[
			"50m67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"50m".ToTiles(),
					[
						new Pair("50m".ToTiles()),
					]),
					"67z".ToTiles()),
			},
			"there are two number suit fives with one red five",
		];
		yield return
		[
			"55m67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55m".ToTiles(),
					[
						new Pair("55m".ToTiles()),
					]),
					"67z".ToTiles()),
			},
			"there are two number suit fives with no red fives",
		];
		yield return
		[
			"5m1677z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"77z".ToTiles(),
					[
						new Pair("77z".ToTiles()),
					]),
					"5m16z".ToTiles()),
			},
			"there is one number suit non-red five with some other pair",
		];
		yield return
		[
			"0m1677z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"77z".ToTiles(),
					[
						new Pair("77z".ToTiles()),
					]),
					"0m16z".ToTiles()),
			},
			"there is one number suit red five with some other pair",
		];
		yield return
		[
			"1550m1555s155z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55m55s55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15s1z".ToTiles()),
				new(new SevenPairsWait(
					"50m55s55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s1z".ToTiles()),
			},
			"there are pairs of fives in two number suits, with a red five in one suit",
		];
		yield return
		[
			"1550s1550p155z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55s55p55z".ToTiles(),
					[
						new Pair("55s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10s10p1z".ToTiles()),
				new(new SevenPairsWait(
					"50s55p55z".ToTiles(),
					[
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15s10p1z".ToTiles()),
				new(new SevenPairsWait(
					"55s50p55z".ToTiles(),
					[
						new Pair("55s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10s15p1z".ToTiles()),
				new(new SevenPairsWait(
					"50s50p55z".ToTiles(),
					[
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15s15p1z".ToTiles()),
			},
			"there are pairs of fives in two number suits (sou-pin), with a red five in both suits",
		];
		yield return
		[
			"1550m1550p155z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55m55p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m10p1z".ToTiles()),
				new(new SevenPairsWait(
					"50m55p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m10p1z".ToTiles()),
				new(new SevenPairsWait(
					"55m50p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15p1z".ToTiles()),
				new(new SevenPairsWait(
					"50m50p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15p1z".ToTiles()),
			},
			"there are pairs of fives in two number suits (man-pin), with a red five in both suits",
		];
		yield return
		[
			"1550m1550s1550p55z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55m55s55p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m10s10p".ToTiles()),
				new(new SevenPairsWait(
					"50m55s55p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m10s10p".ToTiles()),
				new(new SevenPairsWait(
					"55m50s55p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15s10p".ToTiles()),
				new(new SevenPairsWait(
					"50m50s55p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s10p".ToTiles()),
				new(new SevenPairsWait(
					"55m55s50p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m10s15p".ToTiles()),
				new(new SevenPairsWait(
					"50m55s50p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m10s15p".ToTiles()),
				new(new SevenPairsWait(
					"55m50s50p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15s15p".ToTiles()),
				new(new SevenPairsWait(
					"50m50s50p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s15p".ToTiles()),
			},
			"there are pairs of fives in all number suits, all with red fives",
		];
	}

	private static IEnumerable<object[]> SevenPairsWaitGetPossibleForTileTestCases()
	{
		yield return
		[
			"0p",
			"55p67z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"50p".ToTiles(),
					[
						new Pair("50p".ToTiles()),
					]),
					"5p67z".ToTiles()),
			},
			"there are three number suit fives with one red five (as desired tile)",
		];
		yield return
		[
			"0m",
			"155m1555s155z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"50m55s55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("55s".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s1z".ToTiles()),
			},
			"there are pairs of fives in two number suits, with a red five in one suit (as desired tile)",
		];
		yield return
		[
			"0p",
			"1550s155p155z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55s50p55z".ToTiles(),
					[
						new Pair("55s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10s15p1z".ToTiles()),
				new(new SevenPairsWait(
					"50s50p55z".ToTiles(),
					[
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15s15p1z".ToTiles()),
			},
			"there are pairs of fives in two number suits (sou-pin), with a red five in both suits, and the red pin desired",
		];
		yield return
		[
			"0s",
			"1550m155s1550p55z",
			new List<MadeBlockContext>()
			{
				new(new SevenPairsWait(
					"55m50s55p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15s10p".ToTiles()),
				new(new SevenPairsWait(
					"50m50s55p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s10p".ToTiles()),
				new(new SevenPairsWait(
					"55m50s50p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"10m15s15p".ToTiles()),
				new(new SevenPairsWait(
					"50m50s50p55z".ToTiles(),
					[
						new Pair("50m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("50p".ToTiles()),
						new Pair("55z".ToTiles()),
					]),
					"15m15s15p".ToTiles()),
			},
			"there are pairs of fives in all number suits, all with red fives, and the red sou desired",
		];
	}

	private static IEnumerable<object[]> ThirteenOrphansWaitEqualsTestCases()
	{
		yield return
		[
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
				]
			),
			null,
			false,
			"null should not equal",
		];
		yield return
		[
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
				]
			),
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
				]
			),
			true,
			"they have the same tiles and pairs",
		];
		yield return
		[
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
				]
			),
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("19p".ToTiles()),
					new Pair("19p".ToTiles()),
				]
			),
			false,
			"pairs created should matter",
		];
		yield return
		[
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
				]
			),
			new ThirteenOrphansWait(
				"19m19s1199p123457".ToTiles(),
				[
					new Pair("11p".ToTiles()),
					new Pair("99p".ToTiles()),
					new Pair("55z".ToTiles()),
				]
			),
			false,
			"count of pairs should matter",
		];
	}

	private static IEnumerable<object[]> ThirteenOrphansWaitGetPossibleTestCases()
	{
		yield return ["25p36s47m", new List<MadeBlockContext>(), "there are no terminals or honors"];
		yield return
		[
			"25p16s47m",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("1s".ToTiles(), []), "25p6s47m".ToTiles()),
			},
			"there is a single one terminal",
		];
		yield return
		[
			"259p36s47m",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("9p".ToTiles(), []), "25p36s47m".ToTiles()),
			},
			"there is a single nine terminal",
		];
		yield return
		[
			"25p36s47m3z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("3z".ToTiles(), []), "25p36s47m".ToTiles()),
			},
			"there is a single honor",
		];
		yield return
		[
			"1259p36s479m",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("19p9m".ToTiles(), []), "25p36s47m".ToTiles()),
			},
			"there are multiple terminals across multiple suits, but no pairs",
		];
		yield return
		[
			"25p36s47m2367z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("2367z".ToTiles(), []), "25p36s47m".ToTiles()),
			},
			"there are multiple honors, but no pairs",
		];
		yield return
		[
			"25p136s1479m2367z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait("1s19m2367z".ToTiles(), []), "25p36s47m".ToTiles()),
			},
			"there are both terminals and honors, but no pairs",
		];
		yield return
		[
			"25p1136s1479m2367z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait(
					"11s19m2367z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]),
					"25p36s47m".ToTiles()),
			},
			"there are both terminals and honors, with a single pair",
		];
		yield return
		[
			"11p11s199m223667z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait(
					"11p11s199m223667z".ToTiles(),
					[
						new Pair("11p".ToTiles()),
						new Pair("11s".ToTiles()),
						new Pair("99m".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("66z".ToTiles()),
					]),
					[]),
			},
			"there are both terminals and honors, with multiple pairs",
		];
		yield return
		[
			"1111p11s19m23667z",
			new List<MadeBlockContext>()
			{
				new(new ThirteenOrphansWait(
					"11p11s19m23667z".ToTiles(),
					[
						new Pair("11p".ToTiles()),
						new Pair("11s".ToTiles()),
						new Pair("66z".ToTiles()),
					]),
					"11p".ToTiles()),
			},
			"there are both terminals and honors, with multiple pairs, one of which has redundant extras",
		];
	}
}