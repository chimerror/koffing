using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
[RequireGodotRuntime]
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
			"3m",
			"1245m",
			new List<MadeBlockContext>()
			{
				new(new Chow("123m".ToTiles()), "45m".ToTiles()),
				new(new Chow("234m".ToTiles()), "15m".ToTiles()),
				new(new Chow("345m".ToTiles()), "12m".ToTiles()),
			},
			"there are multiple possible chows with no five tiles",
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
}