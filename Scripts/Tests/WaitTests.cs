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
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
				AssertThat(orphan.Tiles.Single()).IsEqual(tile);
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
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(RyanmenGetPossibleTestCases))]
	public static void RyanmenGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(RyanmenGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Ryanmen.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Ryanmen.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(KanchanGetPossibleTestCases))]
	public static void KanchanGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(KanchanGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Kanchan.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Kanchan.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(PenchanGetPossibleTestCases))]
	public static void PenchanGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(PenchanGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Penchan.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Penchan.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
}