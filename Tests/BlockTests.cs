using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
[RequireGodotRuntime]
public class BlockTests
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
	[DataPoint(nameof(PungGetPossibleForTileTestCases))]
	public static void PungGetPossibleForTileIsCorrect(
		string tileNotation,
		string otherTilesNotation,
		List<MadeBlockContext> expectedOutput,
		string because)
	{
		LoggingPrefix = nameof(PungGetPossibleForTileIsCorrect);

		var tile = tileNotation.ToAutoFreeTile();
		var otherTiles = otherTilesNotation.ToAutoFreeTiles();
		var actualOutput = Pung.GetPossibleForTile(tile, otherTiles).ToList();
		PrefixInfo($"Checking that Pung.GetPossibleForTile with tile \"{tileNotation}\" and other tiles \"{otherTilesNotation}\" is correct when {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
				new(new Pung("777p".ToAutoFreeTiles()), "7p3s11m66z".ToAutoFreeTiles()),
			},
			"there are more than enough matching pin tiles",
		];
		yield return
		[
			"4s",
			"067p1234456s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("444s".ToAutoFreeTiles()), "067p12356s11m66z".ToAutoFreeTiles()),
			},
			"there are enough matching sou tiles",
		];
		yield return
		[
			"1m",
			"2p3s11m66z",
			new List<MadeBlockContext>()
			{
				new(new Pung("111m".ToAutoFreeTiles()), "2p3s66z".ToAutoFreeTiles()),
			},
			"there are enough matching man tiles",
		];
		yield return
		[
			"5z",
			"2p3s11m55z",
			new List<MadeBlockContext>()
			{
				new(new Pung("555z".ToAutoFreeTiles()), "2p3s11m".ToAutoFreeTiles()),
			},
			"there are enough matching zi tiles",
		];
		yield return
		[
			"0m",
			"2p3s55m77z",
			new List<MadeBlockContext>()
			{
				new(new Pung("055m".ToAutoFreeTiles()), "2p3s77z".ToAutoFreeTiles()),
			},
			"given red five and there are enough matching tiles",
		];
		yield return
		[
			"5s",
			"067p12344550s11m66z",
			new List<MadeBlockContext>() {
				new(new Pung("555s".ToAutoFreeTiles()), "067p12344s11m66z0s".ToAutoFreeTiles()),
				new(new Pung("550s".ToAutoFreeTiles()), "067p12344s11m66z5s".ToAutoFreeTiles()),
			},
			"given non-red five and there are more than enough matching tiles with a red five",
		];
	}
}