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
}