using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class MeldTests
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
	[DataPoint(nameof(KongGetPossibleForTileTestCases))]
	public static void KongGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(KongGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Kong.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Kong.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(PungGetPossibleForTileTestCases))]
	public static void PungGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(PungGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Pung.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Pung.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(ChowGetPossibleForTileTestCases))]
	public static void ChowGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(ChowGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Chow.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Chow.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(ChowGetPossibleTestCases))]
	public static void ChowGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(ChowGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Chow.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Chow.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	private static IEnumerable<object[]> KongGetPossibleForTileTestCases()
	{
		yield return ["5z", "2p3s11m55z", new List<MadeBlockContext>(), "there are only two matching tiles"];
		yield return ["5z", "2p3s11m5z", new List<MadeBlockContext>(), "there is only one matching tile"];
		yield return ["5z", "2p3s11m", new List<MadeBlockContext>(), "there are no matching tiles"];
		yield return
		[
			"7p",
			"777p2s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Kong("7777p".ToTiles()), "2s11m66z".ToTiles()),
			},
			"there are enough matching pin tiles",
		];
		yield return
		[
			"4s",
			"067p12344456s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Kong("4444s".ToTiles()), "067p12356s11m66z".ToTiles()),
			},
			"there are enough matching sou tiles",
		];
		yield return
		[
			"1m",
			"2p3s111m66z",
			new List<MadeBlockContext>()
			{
				new(new Kong("1111m".ToTiles()), "2p3s66z".ToTiles()),
			},
			"there are enough matching man tiles",
		];
		yield return
		[
			"5z",
			"2p3s11m555z",
			new List<MadeBlockContext>()
			{
				new(new Kong("5555z".ToTiles()), "2p3s11m".ToTiles()),
			},
			"there are enough matching zi tiles",
		];
		yield return
		[
			"0m",
			"2p3s555m77z",
			new List<MadeBlockContext>()
			{
				new(new Kong("0555m".ToTiles()), "2p3s77z".ToTiles()),
			},
			"given red five and there are enough matching tiles",
		];
		yield return
		[
			"5m",
			"2p3s550m77z",
			new List<MadeBlockContext>()
			{
				new(new Kong("0555m".ToTiles()), "2p3s77z".ToTiles()),
			},
			"given non-red five and there are enough matching tiles",
		];
	}

	private static IEnumerable<object[]> PungGetPossibleForTileTestCases()
	{
		yield return ["5z", "2p3s11m5z", new List<MadeBlockContext>(), "there is only one matching tile"];
		yield return ["5z", "2p3s11m", new List<MadeBlockContext>(), "there are no matching tiles"];
		yield return
		[
			"7p",
			"777p3s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("777p".ToTiles()), "7p3s11m66z".ToTiles()),
			},
			"there are more than enough matching pin tiles",
		];
		yield return
		[
			"4s",
			"067p1234456s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("444s".ToTiles()), "067p12356s11m66z".ToTiles()),
			},
			"there are enough matching sou tiles",
		];
		yield return
		[
			"1m",
			"2p3s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("111m".ToTiles()), "2p3s66z".ToTiles()),
			},
			"there are enough matching man tiles",
		];
		yield return
		[
			"5z",
			"2p3s11m55z",
			new List<MadeBlockContext>()
			{
				new(new Pung("555z".ToTiles()), "2p3s11m".ToTiles()),
			},
			"there are enough matching zi tiles",
		];
		yield return
		[
			"0m",
			"2p3s55m77z",
			new List<MadeBlockContext>()
			{
				new(new Pung("055m".ToTiles()), "2p3s77z".ToTiles()),
			},
			"given red five and there are enough matching tiles",
		];
		yield return
		[
			"5s",
			"067p12344550s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("555s".ToTiles()), "067p12344s11m66z0s".ToTiles()),
				new(new Pung("550s".ToTiles()), "067p12344s11m66z5s".ToTiles()),
			},
			"given non-red five and there are more than enough matching tiles with a red five",
		];
	}

	private static IEnumerable<object[]> ChowGetPossibleForTileTestCases()
	{
		yield return ["5z", "2p3s11m34z", new List<MadeBlockContext>(), "given an honor tile with sequential neighbors"];
		yield return ["7p", "46p3s1134z", new List<MadeBlockContext>(), "only one relevant tile is available"];
		yield return [
			"1m",
			"2345m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "45m".ToTiles()),
			},
			"there is a single possible chow on a one",
		];
		yield return [
			"2m",
			"1345m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "45m".ToTiles()),
				new(new Chow("234m".ToTiles()), "15m".ToTiles()),
			},
			"there are two possible chows on a two",
		];
		yield return [
			"8s",
			"5679s",
			new List<MadeBlockContext>()
			{
				new(new Chow("678s".ToTiles()), "59s".ToTiles()),
				new(new Chow("789s".ToTiles()), "56s".ToTiles()),
			},
			"there are two possible chows on a two",
		];
		yield return [
			"9p",
			"5678p",
			new List<MadeBlockContext>()
			{
				new(new Chow("789p".ToTiles()), "56p".ToTiles()),
			},
			"there is a single possible chow on a nine",
		];
		yield return [
			"3m",
			"1245m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "45m".ToTiles()),
				new(new Chow("234m".ToTiles()), "15m".ToTiles()),
				new(new Chow("345m".ToTiles()), "12m".ToTiles()),
			},
			"there are multiple possible chows for a three (and other tiles at least 2 away from a terminal)",
		];
		yield return [
			"4s",
			"34067s",
			new List<MadeBlockContext>()
			{
				new(new Chow("340s".ToTiles()), "467s".ToTiles()),
				new(new Chow("406s".ToTiles()), "347s".ToTiles()),
			},
			"multiple chows can be made using a red five from the other tiles",
		];
		yield return [
			"0p",
			"34567p",
			new List<MadeBlockContext>()
			{
				new(new Chow("340p".ToTiles()), "567p".ToTiles()),
				new(new Chow("406p".ToTiles()), "357p".ToTiles()),
				new(new Chow("067p".ToTiles()), "345p".ToTiles()),
			},
			"multiple chows can be made using a chosen red five",
		];
	}

	private static IEnumerable<object[]> ChowGetPossibleTestCases()
	{
		yield return [
			"12345s",
			new List<MadeBlockContext>()
			{
				new(new Chow("123s".ToTiles()), "45s".ToTiles()),
				new(new Chow("234s".ToTiles()), "15s".ToTiles()),
				new(new Chow("345s".ToTiles()), "12s".ToTiles()),
			},
			"there are five tiles in a row with no red fives",
		];
		yield return [
			"123450s",
			new List<MadeBlockContext>()
			{
				new(new Chow("123s".ToTiles()), "450s".ToTiles()),
				new(new Chow("234s".ToTiles()), "150s".ToTiles()),
				new(new Chow("345s".ToTiles()), "120s".ToTiles()),
				new(new Chow("340s".ToTiles()), "125s".ToTiles()),
			},
			"there are five tiles in a row with an extra red five",
		];
		yield return [
			"123567m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "567m".ToTiles()),
				new(new Chow("567m".ToTiles()), "123m".ToTiles()),
			},
			"there are two disconnected chows",
		];
		yield return [
			"456m456s456p",
			new List<MadeBlockContext>()
			{
				new(new Chow("456m".ToTiles()), "456s456p".ToTiles()),
				new(new Chow("456s".ToTiles()), "456m456p".ToTiles()),
				new(new Chow("456p".ToTiles()), "456m456s".ToTiles()),
			},
			"there are three chows of the same ranks of different suits",
		];
		yield return [
			"122345s",
			new List<MadeBlockContext>()
			{
				new(new Chow("123s".ToTiles()), "245s".ToTiles()),
				new(new Chow("234s".ToTiles()), "125s".ToTiles()),
				new(new Chow("345s".ToTiles()), "122s".ToTiles()),
			},
			"there are five tiles in a row with an extra non red-five tile",
		];
		yield return [
			"1122334455m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "1234455m".ToTiles()),
				new(new Chow("234m".ToTiles()), "1123455m".ToTiles()),
				new(new Chow("345m".ToTiles()), "1122345m".ToTiles()),
			},
			"there are five ranks of duplicate tiles in a row",
		];
	}
}