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
	[DataPoint(nameof(GetFirstLevelMeldsTestCases))]
	public static void GetFirstLevelMeldsIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(GetFirstLevelMeldsIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Meld.GetFirstLevelMelds(tiles);
		PrefixInfo($"Checking that Meld.GetFirstLevelMelds with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(PairGetPossibleForTileTestCases))]
	public static void PairGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(PairGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = Pair.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Pair.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	[TestCase]
	[DataPoint(nameof(PairGetPossibleTestCases))]
	public static void PairGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(PairGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Pair.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Pair.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	// These PairWait tests might should be in WaitTests, but since they are going to just use the Pair test cases, it's
	// easier to put them here.
	[TestCase]
	[DataPoint(nameof(PairGetPossibleForTileTestCases))]
	public static void PairWaitGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(PairWaitGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToTile();
		var otherTiles = otherTilesNotation.ToTiles();
		var actualOutput = PairWait.GetPossibleForTile(tile, otherTiles).ToList();
		var convertedExpected = expectedOutput
			.Select(c => new MadeBlockContext(new PairWait(c.MadeBlock.Tiles), c.RemainingTiles))
			.ToList();

		PrefixInfo($"Checking that PairWait.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(convertedExpected);
	}

	[TestCase]
	[DataPoint(nameof(PairGetPossibleTestCases))]
	public static void PairWaitGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(PairGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = PairWait.GetPossible(tiles).ToList();
		var convertedExpected = expectedOutput
			.Select(c => new MadeBlockContext(new PairWait(c.MadeBlock.Tiles), c.RemainingTiles))
			.ToList();

		PrefixInfo($"Checking that PairWait.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(convertedExpected);
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
	[DataPoint(nameof(KongGetPossibleTestCases))]
	public static void KongGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(KongGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Kong.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Kong.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
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
	[DataPoint(nameof(PungGetPossibleTestCases))]
	public static void PungGetPossibleIsCorrect(string tilesNotation, List<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(PungGetPossibleIsCorrect);

		var tiles = tilesNotation.ToTiles();
		var actualOutput = Pung.GetPossible(tiles).ToList();
		PrefixInfo($"Checking that Pung.GetPossible with tiles \"{tilesNotation}\" is correct when {because}");
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

	private static IEnumerable<object[]> GetFirstLevelMeldsTestCases()
	{
		yield return
		[
			"1111234s",
			new List<MadeBlockContext>()
			{
				new(new Chow("123s".ToTiles()), "1114s".ToTiles()),
				new(new Chow("234s".ToTiles()), "1111s".ToTiles()),
				new(new Pung("111s".ToTiles()), "1234s".ToTiles()),
				new(new Kong("1111s".ToTiles()), "234s".ToTiles()),
			},
			"there are melds of each type in one suit",
		];
		yield return
		[
			"1111234s1111234p",
			new List<MadeBlockContext>()
			{
				new(new Chow("123s".ToTiles()), "1114s1111234p".ToTiles()),
				new(new Chow("123p".ToTiles()), "1114p1111234s".ToTiles()),
				new(new Chow("234s".ToTiles()), "1111s1111234p".ToTiles()),
				new(new Chow("234p".ToTiles()), "1111p1111234s".ToTiles()),
				new(new Pung("111s".ToTiles()), "1234s1111234p".ToTiles()),
				new(new Pung("111p".ToTiles()), "1234p1111234s".ToTiles()),
				new(new Kong("1111s".ToTiles()), "234s1111234p".ToTiles()),
				new(new Kong("1111p".ToTiles()), "234p1111234s".ToTiles()),
			},
			"there are melds of each type in two suits",
		];
		yield return
		[
			"1112345678999m",
			new List<MadeBlockContext>()
			{
				new(new Pung("111m".ToTiles()), "2345678999m".ToTiles()),
				new(new Chow("123m".ToTiles()), "1145678999m".ToTiles()),
				new(new Chow("234m".ToTiles()), "1115678999m".ToTiles()),
				new(new Chow("345m".ToTiles()), "1112678999m".ToTiles()),
				new(new Chow("456m".ToTiles()), "1112378999m".ToTiles()),
				new(new Chow("567m".ToTiles()), "1112348999m".ToTiles()),
				new(new Chow("678m".ToTiles()), "1112345999m".ToTiles()),
				new(new Chow("789m".ToTiles()), "1112345699m".ToTiles()),
				new(new Pung("999m".ToTiles()), "1112345678m".ToTiles()),
			},
			"the hand is true nine gates",
		];
	}

	private static IEnumerable<object[]> PairGetPossibleForTileTestCases()
	{
		yield return ["5z", "2p3s11m", new List<MadeBlockContext>(), "there are no matching tiles"];
		yield return
		[
			"5z",
			"2p3s11m5z",
			new List<MadeBlockContext>()
			{
				new(new Pair("55z".ToTiles()), "2p3s11m".ToTiles()),
			},
			"there is one matching tile"
		];
		yield return
		[
			"5z",
			"2p3s11m555z",
			new List<MadeBlockContext>()
			{
				new(new Pair("55z".ToTiles()), "2p3s11m55z".ToTiles()),
			},
			"there are redundant matching tiles"
		];
		yield return
		[
			"5m",
			"2p3s1155m",
			new List<MadeBlockContext>()
			{
				new(new Pair("55m".ToTiles()), "2p3s115m".ToTiles()),
			},
			"there is a redundant non-red five"
		];
		yield return
		[
			"0m",
			"2p3s115m",
			new List<MadeBlockContext>()
			{
				new(new Pair("05m".ToTiles()), "2p3s11m".ToTiles()),
			},
			"there is a non-red five to pair with a red five"
		];
		yield return
		[
			"5m",
			"2p3s110m",
			new List<MadeBlockContext>()
			{
				new(new Pair("50m".ToTiles()), "2p3s11m".ToTiles()),
			},
			"there is a matching red five"
		];
		yield return
		[
			"5m",
			"2p3s1150m",
			new List<MadeBlockContext>()
			{
				new(new Pair("50m".ToTiles()), "2p3s115m".ToTiles()),
				new(new Pair("55m".ToTiles()), "2p3s110m".ToTiles()),
			},
			"there is a matching red five AND a matching non-red five"
		];
	}

	private static IEnumerable<object[]> PairGetPossibleTestCases()
	{
		yield return ["2p3s1m4z", new List<MadeBlockContext>(), "there are no matching tiles"];
		yield return
		[
			"2p3s11m5z",
			new List<MadeBlockContext>()
			{
				new(new Pair("11m".ToTiles()), "2p3s5z".ToTiles()),
			},
			"there is only one pair",
		];
		yield return
		[
			"2p3s1177m5z",
			new List<MadeBlockContext>()
			{
				new(new Pair("11m".ToTiles()), "2p3s77m5z".ToTiles()),
				new(new Pair("77m".ToTiles()), "2p3s11m5z".ToTiles()),
			},
			"there are multiple pairs in the same suit",
		];
		yield return
		[
			"2p3s1177m5z",
			new List<MadeBlockContext>()
			{
				new(new Pair("11m".ToTiles()), "2p3s77m5z".ToTiles()),
				new(new Pair("77m".ToTiles()), "2p3s11m5z".ToTiles()),
			},
			"there are multiple pairs in the same suit",
		];
		yield return
		[
			"2p3s1177m55z",
			new List<MadeBlockContext>()
			{
				new(new Pair("11m".ToTiles()), "2p3s77m55z".ToTiles()),
				new(new Pair("77m".ToTiles()), "2p3s11m55z".ToTiles()),
				new(new Pair("55z".ToTiles()), "2p3s1177m".ToTiles()),
			},
			"there are multiple pairs in different suits",
		];
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

	private static IEnumerable<object[]> KongGetPossibleTestCases()
	{
		yield return [
			"11112223333z",
			new List<MadeBlockContext>()
			{
				new(new Kong("1111z".ToTiles()), "2223333z".ToTiles()),
				new(new Kong("3333z".ToTiles()), "1111222z".ToTiles()),
			},
			"there are multiple kongs in the same suit",
		];
		yield return [
			"1111s222p3333z",
			new List<MadeBlockContext>()
			{
				new(new Kong("1111s".ToTiles()), "222p3333z".ToTiles()),
				new(new Kong("3333z".ToTiles()), "1111s222p".ToTiles()),
			},
			"there are multiple kongs in different suits",
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

	private static IEnumerable<object[]> PungGetPossibleTestCases()
	{
		yield return [
			"111222333z",
			new List<MadeBlockContext>()
			{
				new(new Pung("111z".ToTiles()), "222333z".ToTiles()),
				new(new Pung("222z".ToTiles()), "111333z".ToTiles()),
				new(new Pung("333z".ToTiles()), "111222z".ToTiles()),
			},
			"there are multiple pungs in the same suit",
		];
		yield return [
			"111s222p333z",
			new List<MadeBlockContext>()
			{
				new(new Pung("111s".ToTiles()), "222p333z".ToTiles()),
				new(new Pung("222p".ToTiles()), "111s333z".ToTiles()),
				new(new Pung("333z".ToTiles()), "111s222p".ToTiles()),
			},
			"there are multiple pungs in different suits",
		];
		yield return [
			"111s2222p333z",
			new List<MadeBlockContext>()
			{
				new(new Pung("111s".ToTiles()), "2222p333z".ToTiles()),
				new(new Pung("222p".ToTiles()), "111s2p333z".ToTiles()),
				new(new Pung("333z".ToTiles()), "111s2222p".ToTiles()),
			},
			"there is a kong",
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