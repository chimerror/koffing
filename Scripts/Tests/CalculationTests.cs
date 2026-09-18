using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static Readiness;
using static TestLoggingHelpers;

[TestSuite]
public class CalculationTests
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
	[DataPoint(nameof(IsReadyTestCases))]
	public static void IsReadyIsCorrect(BlockSet hand, bool expectedResult, Readiness.Type expectedReadinessType, string because)
	{
		LoggingPrefix = nameof(IsReadyIsCorrect);

		var handTiles = hand.Blocks.SelectMany(b => b.Tiles).ToList();

		var outcomeString = expectedResult ? "is ready" : "is NOT ready";
		PrefixInfo($"Checking that hand \"{handTiles.NotationFromTiles()}\" {outcomeString} given {because}");
		AssertThat(hand.IsReady(out var actualReadinessType)).IsEqual(expectedResult);
		PrefixInfo($"Checking that readiness type {expectedReadinessType} was used to determine readiness.");
		AssertThat(actualReadinessType).IsEqual(expectedReadinessType);
	}

	[TestCase]
	[DataPoint(nameof(IsPastReadyTestCases))]
	public static void IsPastReadyIsCorrect(BlockSet hand, bool expectedResult, Readiness.Type expectedReadinessType, string because)
	{
		LoggingPrefix = nameof(IsReadyIsCorrect);

		var handTiles = hand.Blocks.SelectMany(b => b.Tiles).ToList();

		var outcomeString = expectedResult ? "is past-ready" : "is NOT past-ready";
		PrefixInfo($"Checking that hand \"{handTiles.NotationFromTiles()}\" {outcomeString} given {because}");
		AssertThat(hand.IsPastReady(out var actualReadinessType)).IsEqual(expectedResult);
		PrefixInfo($"Checking that readiness type {expectedReadinessType} was used to determine readiness.");
		AssertThat(actualReadinessType).IsEqual(expectedReadinessType);
	}

	[TestCase]
	[DataPoint(nameof(StandardTilesToReadyTestCases))]
	public static void StandardTilesToReadyIsCorrect(BlockSet hand, int expectedResult, string because)
	{
		LoggingPrefix = nameof(StandardTilesToReadyIsCorrect);

		var handTiles = hand.Blocks.SelectMany(b => b.Tiles).ToList();

		PrefixInfo($"Checking that hand \"{handTiles.NotationFromTiles()}\" has standard readiness calculated as {expectedResult} given that it's {because}");
		AssertThat(StandardTilesToReady(hand)).IsEqual(expectedResult);
	}

	[TestCase]
	[ThrowsException(typeof(InvalidOperationException))]
	[DataPoint(nameof(StandardTilesToReadyEdgeTestCases))]
	public static void StandardTilesToReadyEdgeCasesAreCorrect(BlockSet hand, string because)
	{
		LoggingPrefix = nameof(StandardTilesToReadyEdgeCasesAreCorrect);

		PrefixInfo($"Checking that StandardTilesToReady throws an InvalidOperationException when given {because}");
		StandardTilesToReady(hand); // Will throw
	}

	[TestCase]
	[DataPoint(nameof(SevenPairsTilesToReadyTestCases))]
	public static void SevenPairsTilesToReadyIsCorrect(BlockSet hand, int expectedResult, string because)
	{
		LoggingPrefix = nameof(SevenPairsTilesToReadyIsCorrect);

		var handTiles = hand.Blocks.SelectMany(b => b.Tiles).ToList();

		PrefixInfo($"Checking that hand \"{handTiles.NotationFromTiles()}\" has seven pairs readiness calculated as {expectedResult} given that it's {because}");
		AssertThat(SevenPairsTilesToReady(hand)).IsEqual(expectedResult);
	}

	[TestCase]
	[ThrowsException(typeof(InvalidOperationException))]
	[DataPoint(nameof(SevenPairsTilesToReadyEdgeTestCases))]
	public static void SevenPairsTilesToReadyEdgeCasesAreCorrect(BlockSet hand, string because)
	{
		LoggingPrefix = nameof(SevenPairsTilesToReadyEdgeCasesAreCorrect);

		PrefixInfo($"Checking that SevenPairsTilesToReady throws an InvalidOperationException when given {because}");
		SevenPairsTilesToReady(hand); // Will throw
	}

	[TestCase]
	[DataPoint(nameof(ThirteenOrphansTilesToReadyTestCases))]
	public static void ThirteenOrphansTilesToReadyIsCorrect(BlockSet hand, int expectedResult, string because)
	{
		LoggingPrefix = nameof(ThirteenOrphansTilesToReadyIsCorrect);

		var handTiles = hand.Blocks.SelectMany(b => b.Tiles).ToList();

		PrefixInfo($"Checking that hand \"{handTiles.NotationFromTiles()}\" has thirteen orphans readiness calculated as {expectedResult} given that it's {because}");
		AssertThat(ThirteenOrphansTilesToReady(hand)).IsEqual(expectedResult);
	}

	[TestCase]
	[ThrowsException(typeof(InvalidOperationException))]
	[DataPoint(nameof(ThirteenOrphansTilesToReadyEdgeTestCases))]
	public static void ThirteenOrphansTilesToReadyEdgeCasesAreCorrect(BlockSet hand, string because)
	{
		LoggingPrefix = nameof(ThirteenOrphansTilesToReadyEdgeCasesAreCorrect);

		PrefixInfo($"Checking that ThirteenOrphansTilesToReady throws an InvalidOperationException when given {because}");
		ThirteenOrphansTilesToReady(hand); // Will throw
	}

	private static IEnumerable<object[]> IsReadyTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Orphan("1m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			false,
			Readiness.Type.Standard,
			"a non-ready fully disconnected standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new PairWait("11z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			false,
			Readiness.Type.Standard,
			"a partially ready (and thus, non-ready) standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Pung("111z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			true,
			Readiness.Type.Standard,
			"a ready standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			false,
			Readiness.Type.Standard,
			"a completed (and thus, past-ready) standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]
				),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			false,
			Readiness.Type.SevenPairs,
			"a non-ready seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"1122334455667z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
						new Pair("55z".ToTiles()),
						new Pair("66z".ToTiles()),
					]
				),
				new Orphan("7z".ToTile()),
			]),
			true,
			Readiness.Type.SevenPairs,
			"a ready seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344556677z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
						new Pair("55z".ToTiles()),
						new Pair("66z".ToTiles()),
						new Pair("77z".ToTiles()),
					]
				),
			]),
			false,
			Readiness.Type.SevenPairs,
			"a completed (and thus, past-ready) seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"11p11s19m23667z".ToTiles(),
					[
						new Pair("11p".ToTiles()),
						new Pair("11s".ToTiles()),
						new Pair("66z".ToTiles()),
					]),
			]),
			false,
			Readiness.Type.ThirteenOrphans,
			"a non-ready thirteen orphans hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"1p119s19m1234567z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]),
			]),
			true,
			Readiness.Type.ThirteenOrphans,
			"a ready thirteen orphans hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"19p119s19m1234567z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]),
			]),
			false,
			Readiness.Type.ThirteenOrphans,
			"a complete (and thus past-ready) thirteen orphans hand",
		];
	}

	private static IEnumerable<object[]> IsPastReadyTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Orphan("1m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			false,
			Readiness.Type.Standard,
			"a non-ready fully disconnected standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new PairWait("11z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			false,
			Readiness.Type.Standard,
			"a partially ready (and thus, non-ready) standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Pung("111z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			false,
			Readiness.Type.Standard,
			"a ready standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("111m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new PairWait("22z".ToTiles()),
			]),
			true,
			Readiness.Type.Standard,
			"a completed (and thus, past-ready) standard hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]
				),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			false,
			Readiness.Type.SevenPairs,
			"a non-ready seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"1122334455667z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
						new Pair("55z".ToTiles()),
						new Pair("66z".ToTiles()),
					]
				),
				new Orphan("7z".ToTile()),
			]),
			false,
			Readiness.Type.SevenPairs,
			"a ready seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344556677z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
						new Pair("55z".ToTiles()),
						new Pair("66z".ToTiles()),
						new Pair("77z".ToTiles()),
					]
				),
			]),
			true,
			Readiness.Type.SevenPairs,
			"a completed (and thus, past-ready) seven pairs hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"11p11s19m23667z".ToTiles(),
					[
						new Pair("11p".ToTiles()),
						new Pair("11s".ToTiles()),
						new Pair("66z".ToTiles()),
					]),
			]),
			false,
			Readiness.Type.ThirteenOrphans,
			"a non-ready thirteen orphans hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"1p119s19m1234567z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]),
			]),
			false,
			Readiness.Type.ThirteenOrphans,
			"a ready thirteen orphans hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"19p119s19m1234567z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]),
			]),
			true,
			Readiness.Type.ThirteenOrphans,
			"a complete (and thus past-ready) thirteen orphans hand",
		];
	}

	private static IEnumerable<object[]> StandardTilesToReadyTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Orphan("1m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			8,
			"a fully disconnected hand"
		];
		yield return
		[
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			6,
			"a hand with only one complete meld"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Orphan("9m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			7,
			"a hand with only one non-pair wait"
		];
		yield return
		[
			new BlockSet(
			[
				new PairWait("11m".ToTiles()),
				new Orphan("9m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			7,
			"a hand with only one pair wait"
		];
		yield return
		[
			new BlockSet(
			[
				new Pair("11m".ToTiles()),
				new Orphan("9m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			7,
			"a hand with only one pair meld"
		];
		yield return
		[
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Pung("111z".ToTiles()),
				new Orphan("4z".ToTile()),
			]),
			2,
			"a hand with multiple non-pair melds"
		];
		yield return
		[
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new Orphan("4z".ToTile()),
			]),
			0,
			"a hand with four non-pair melds"
		];
		yield return
		[
			// Not technically possible, but worth covering
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new Pung("444z".ToTiles()),
			]),
			0,
			"a hand with non-counted melds past the fourth"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Orphan("9m".ToTile()),
				new Kanchan("24s".ToTiles()),
				new Orphan("8s".ToTile()),
				new Ryanmen("34p".ToTiles()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			5,
			"a hand with multiple non-pair waits"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Ryanmen("56m".ToTiles()),
				new Kanchan("24s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Kanchan("78p".ToTiles()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			4,
			"a hand with non-counted waits past the fourth"
		];
		yield return
		[
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new Pair("44z".ToTiles()),
			]),
			-1,
			"a hand with four non-pair melds and a pair"
		];
		yield return
		[
			// Not technically possible, but worth covering
			new BlockSet(
			[
				new Chow("123m".ToTiles()),
				new Chow("234s".ToTiles()),
				new Chow("345p".ToTiles()),
				new Pung("111z".ToTiles()),
				new Pair("44z".ToTiles()),
				new Pair("55z".ToTiles()),
			]),
			-1,
			"a hand with four non-pair melds and two pairs"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Ryanmen("56m".ToTiles()),
				new Kanchan("24s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Pair("22z".ToTiles()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
				new Orphan("5z".ToTile()),
			]),
			3,
			"a hand with four non-pair waits and a pair"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Ryanmen("56m".ToTiles()),
				new Kanchan("24s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Pair("22z".ToTiles()),
				new Pair("33z".ToTiles()),
				new Orphan("5z".ToTile()),
			]),
			3,
			"a hand with four non-pair waits and two pairs"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Ryanmen("56m".ToTiles()),
				new Kanchan("24s".ToTiles()),
				new Ryanmen("78s".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Pair("22z".ToTiles()),
				new Orphan("5z".ToTile()),
			]),
			3,
			"a hand with five non-pair waits and a pair"
		];
		yield return
		[
			new BlockSet(
			[
				new Penchan("12m".ToTiles()),
				new Ryanmen("56m".ToTiles()),
				new Kanchan("24s".ToTiles()),
				new Pair("22z".ToTiles()),
				new Ryanmen("34p".ToTiles()),
				new Orphan("5z".ToTile()),
				new Orphan("6z".ToTile()),
				new Orphan("7z".ToTile()),
			]),
			3,
			"a hand where the pair is the fourth block"
		];
	}

	private static IEnumerable<object[]>StandardTilesToReadyEdgeTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"55m50s55p55z".ToTiles(),
					[
						new Pair("55m".ToTiles()),
						new Pair("50s".ToTiles()),
						new Pair("55p".ToTiles()),
						new Pair("55z".ToTiles()),
					]
				),
			]),
			"a hand with a seven pairs wait",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"11s19m2367z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]
				),
			]),
			"a hand with a thirteen orphans wait",
		];
	}

	private static IEnumerable<object[]> SevenPairsTilesToReadyTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait("44z".ToTiles(), [new Pair("44z".ToTiles())]),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
			]),
			5,
			"a hand with only one pair"
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]
				),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			2,
			"a hand with multiple pairs"
		];
	}

	private static IEnumerable<object[]>SevenPairsTilesToReadyEdgeTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Orphan("1m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			"a fully disconnected hand",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"11s19m2367z".ToTiles(),
					[
						new Pair("11s".ToTiles()),
					]
				),
			]),
			"a hand with a thirteen orphans wait",
		];
	}

	private static IEnumerable<object[]> ThirteenOrphansTilesToReadyTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait("1p".ToTiles(), []),
				new Orphan("4m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
			]),
			12,
			"a hand with a single honor/terminal tile",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait("1p1s19m2367z".ToTiles(), []),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			5,
			"a hand with no pairs but multiple honor/terminal tiles",
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait("44z".ToTiles(), [new Pair("44z".ToTiles())]),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
			]),
			11,
			"a hand with only one distinct honor/terminal tile and one pair"
		];
		yield return
		[
			new BlockSet(
			[
				new ThirteenOrphansWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]
				),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			8,
			"a hand with multiple pairs"
		];
		yield return
		[
			new BlockSet(
			[
				// Would not be returned by GetPossible, but just to defensively test
				new ThirteenOrphansWait(
					"1111p11s19m23667z".ToTiles(),
					[
						new Pair("11p".ToTiles()),
						new Pair("11s".ToTiles()),
						new Pair("66z".ToTiles()),
					]),
			]),
			4,
			"a hand with redundant honor/terminal tiles",
		];
	}

	private static IEnumerable<object[]>ThirteenOrphansTilesToReadyEdgeTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Orphan("1m".ToTile()),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
				new Orphan("3p".ToTile()),
				new Orphan("6p".ToTile()),
				new Orphan("9p".ToTile()),
				new Orphan("1z".ToTile()),
				new Orphan("2z".ToTile()),
				new Orphan("3z".ToTile()),
				new Orphan("4z".ToTile()),
			]),
			"a fully disconnected hand",
		];
		yield return
		[
			new BlockSet(
			[
				new SevenPairsWait(
					"11223344z".ToTiles(),
					[
						new Pair("11z".ToTiles()),
						new Pair("22z".ToTiles()),
						new Pair("33z".ToTiles()),
						new Pair("44z".ToTiles()),
					]
				),
				new Orphan("4m".ToTile()),
				new Orphan("7m".ToTile()),
				new Orphan("2s".ToTile()),
				new Orphan("5s".ToTile()),
				new Orphan("8s".ToTile()),
			]),
			"a hand with a seven pairs wait",
		];
	}
}