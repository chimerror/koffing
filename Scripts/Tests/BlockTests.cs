using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
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
	[DataPoint(nameof(BlockEqualsEdgeTestCases))]
	public static void BlockEqualsEdgeCasesAreCorrect(Block blockA, object objectB, string because)
	{
		LoggingPrefix = nameof(BlockEqualsEdgeCasesAreCorrect);

		PrefixInfo($"Checking that block A \"{blockA}\" does not equal object B \"{objectB}\" because {because}");
		AssertThat(blockA.Equals(objectB)).IsFalse();
	}

	[TestCase]
	[DataPoint(nameof(BlockEqualsTestCases))]
	public static void BlockEqualsIsCorrect(Block blockA, Block blockB, bool expectedResult, string because)
	{
		LoggingPrefix = nameof(BlockEqualsIsCorrect);

		var outcomeString = expectedResult ? "does equal" : "does NOT equal";
		PrefixInfo($"Checking that block A \"{blockA}\" {outcomeString} block B \"{blockB}\" because {because}");
		AssertThat(blockA.Equals(blockB)).IsEqual(expectedResult);
		if (blockB != null)
		{
			PrefixInfo($"Checking that block B \"{blockB}\" {outcomeString} block A \"{blockA}\" because {because}");
			AssertThat(blockB.Equals(blockA)).IsEqual(expectedResult);
		}
	}

	[TestCase]
	[DataPoint(nameof(GetBlockHashCodeTestCases))]
	public static void GetBlockHashCodeIsCorrect(Block block, int expectedHashCode)
	{
		LoggingPrefix = nameof(GetBlockHashCodeIsCorrect);

		PrefixInfo($"Checking that block of type \"{block.GetType()}\" has hash code {expectedHashCode}");
		AssertThat(block.GetHashCode()).IsEqual(expectedHashCode);
	}

	[TestCase]
	[DataPoint(nameof(CompareBlocksTestCases))]
	public static void CompareBlocksIsCorrect(Block blockA, Block blockB, int expectedComparisonValue, string because)
	{
		LoggingPrefix = nameof(CompareBlocksIsCorrect);

		PrefixInfo($"Checking that Block.CompareTo is {expectedComparisonValue} when {because}");
		AssertThat(blockA.CompareTo(blockB)).IsEqual(expectedComparisonValue);
	}

	[TestCase]
	[DataPoint(nameof(MadeBlockContextEqualsEdgeTestCases))]
	public static void MadeBlockContextEqualsEdgeCasesAreCorrect(MadeBlockContext contextA, object objectB, string because)
	{
		LoggingPrefix = nameof(MadeBlockContextEqualsEdgeCasesAreCorrect);

		PrefixInfo($"Checking that made block context A \"{contextA}\" does not equal object B \"{objectB}\" because {because}");
		AssertThat(contextA.Equals(objectB)).IsFalse();
	}

	[TestCase]
	[DataPoint(nameof(MadeBlockContextEqualsTestCases))]
	public static void MadeBlockContextEqualsCasesAreCorrect(
		MadeBlockContext contextA,
		MadeBlockContext contextB,
		bool expectedResult,
		string because)
	{
		LoggingPrefix = nameof(MadeBlockContextEqualsEdgeCasesAreCorrect);

		var outcomeString = expectedResult ? "does equal" : "does NOT equal";
		PrefixInfo($"Checking that context A \"{contextA}\" {outcomeString} context B \"{contextB}\" because {because}");
		AssertThat(contextA.Equals(contextB)).IsEqual(expectedResult);
		if (contextB != null)
		{
			PrefixInfo($"Checking that context B \"{contextB}\" {outcomeString} context A \"{contextA}\" because {because}");
			AssertThat(contextB.Equals(contextA)).IsEqual(expectedResult);
		}
	}

	[TestCase]
	[DataPoint(nameof(CompareMadeBlockContextsTestCases))]
	public static void CompareMadeBlockContextsIsCorrect(
		MadeBlockContext contextA,
		MadeBlockContext contextB,
		int expectedComparisonValue,
		string because)
	{
		LoggingPrefix = nameof(CompareMadeBlockContextsIsCorrect);

		PrefixInfo($"Checking that MadeBlockContext.CompareTo is {expectedComparisonValue} when comparing {because}");
		AssertThat(contextA.CompareTo(contextB)).IsEqual(expectedComparisonValue);
	}

	[TestCase]
	public static void PairsAndPairWaitsCastingIsCorrect()
	{
		LoggingPrefix = nameof(PairsAndPairWaitsCastingIsCorrect);

		var pair = new Pair("55p".ToTiles());
		var pairWait = new PairWait("55p".ToTiles());

		PrefixInfo($"Checking that pair wait can be cast to pair");
		AssertThat((Pair)pairWait).IsEqual(pair);

		PrefixInfo($"Checking that pair can be cast to pair wait");
		AssertThat((PairWait)pair).IsEqual(pairWait);
	}

	[TestCase]
	[DataPoint(nameof(CompareBlockSetsTestCases))]
	public static void CompareBlockSetsIsCorrect(
		BlockSet blockSetA,
		BlockSet blockSetB,
		int expectedComparisonValue,
		string because)
	{
		LoggingPrefix = nameof(CompareBlockSetsIsCorrect);

		PrefixInfo($"Checking that BlockSet.CompareTo is {expectedComparisonValue} when comparing {because}");
		AssertThat(blockSetA.CompareTo(blockSetB)).IsEqual(expectedComparisonValue);
	}

	[TestCase]
	[DataPoint(nameof(GetPossibleBlockSetsTestCases))]
	public static void GetPossibleBlockSetsIsCorrect(string tilesString, List<BlockSet> expectedOutput, string because)
	{
		LoggingPrefix = nameof(GetPossibleBlockSetsIsCorrect);

		var tiles = tilesString.ToTiles();
		var actualOutput = Block.GetPossibleBlockSets(tiles).Order().ToList();

		PrefixInfo($"Checking that all block sets are returned from a hand with {because}");
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
	}

	private static IEnumerable<object[]> BlockEqualsEdgeTestCases()
	{
		yield return [new Chow("123s".ToTiles()), null, "null should not equal"];
		yield return [new Chow("123s".ToTiles()), "123s", "different types should not equal"];
	}

	private static IEnumerable<object[]> BlockEqualsTestCases()
	{
		// A lot of these may be "incorrect" blocks as far as their actual tiles, but that is not part of the guarantee
		// for the Block Classes. Instead, you should just make sure to use the GetPossible methods to generate them,
		// which will _only_ generate correct blocks. I guess this could bite me in the tail if I do a bad job, but
		// I have already made working tests of the GetPossibleForTile methods.

		yield return
		[
			new Pung("444p".ToTiles()),
			null,
			false,
			"null should not equal",
		];
		yield return
		[
			new Pung("444p".ToTiles()),
			new Chow("444p".ToTiles()),
			false,
			"type should matter",
		];
		yield return
		[
			new Pung("444z".ToTiles()),
			new Pung("44z".ToTiles()),
			false,
			"tile counts should matter",
		];
		yield return
		[
			new Chow("456s".ToTiles()),
			new Chow("456s".ToTiles()),
			true,
			"they have the same type and tiles",
		];
		yield return
		[
			new Chow("456m".ToTiles()),
			new Chow("546m".ToTiles()),
			true,
			"tile order should not matter",
		];
		yield return
		[
			new Chow("456p".ToTiles()),
			new Chow("234p".ToTiles()),
			false,
			"they are the same type but not the same tiles",
		];
	}

	private static IEnumerable<object[]> GetBlockHashCodeTestCases()
	{
		// No tiles in the blocks because that way the exponent remains 1 and we just get the basis, which is what we
		// are really testing here.
		yield return [new Chow(), 2];
		yield return [new Pung(), 3];
		yield return [new Kong(), 5];
		yield return [new Pair(), 7];
		yield return [new Orphan(), 11];
		yield return [new PairWait(), 13];
		yield return [new Ryanmen(), 17];
		yield return [new Kanchan(), 19];
		yield return [new Penchan(), 23];
	}

	private static IEnumerable<object[]> CompareBlocksTestCases()
	{
		// Some of these blocks are "wrong" compared to their names, but this is for the sake of testing.
		yield return [new Chow("123s".ToTiles()), null, 1, "comparing to null block"];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Chow("123s".ToTiles()),
			0,
			"comparing identical Chows",
		];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Chow("312s".ToTiles()),
			0,
			"comparing identical Chows with different orders",
		];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Pung("111z".ToTiles()),
			-1,
			"comparing Chow to Pung",
		];
		yield return
		[
			new Pung("111z".ToTiles()),
			new Chow("123s".ToTiles()),
			1,
			"comparing Pung to Chow",
		];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Kong("4444m".ToTiles()),
			-1,
			"comparing Chow to Kong",
		];
		yield return
		[
			new Kong("4444m".ToTiles()),
			new Chow("123s".ToTiles()),
			1,
			"comparing Kong to Chow",
		];
		yield return
		[
			new Pung("111z".ToTiles()),
			new Pung("111z".ToTiles()),
			0,
			"comparing identical Pungs",
		];
		yield return
		[
			new Pung("111z".ToTiles()),
			new Kong("4444m".ToTiles()),
			-1,
			"comparing Pung to Kong",
		];
		yield return
		[
			new Kong("4444m".ToTiles()),
			new Pung("111z".ToTiles()),
			1,
			"comparing Kong to Pung",
		];
		yield return
		[
			new Kong("4444m".ToTiles()),
			new Kong("4444m".ToTiles()),
			0,
			"comparing identical Kongs",
		];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Chow("1234s".ToTiles()),
			-1,
			"comparing shorter to longer of the same type",
		];
		yield return
		[
			new Chow("1234s".ToTiles()),
			new Chow("123s".ToTiles()),
			1,
			"comparing longer to shorter of the same type",
		];
		yield return
		[
			new Chow("123s".ToTiles()),
			new Chow("234s".ToTiles()),
			-1,
			"comparing earlier to later of the same type",
		];
		yield return
		[
			new Chow("234s".ToTiles()),
			new Chow("123s".ToTiles()),
			1,
			"comparing later to earlier of the same type",
		];
	}

	private static IEnumerable<object[]> MadeBlockContextEqualsEdgeTestCases()
	{
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			null,
			"null should not equal",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			"123s",
			"different types should not equal",
		];
	}

	private static IEnumerable<object[]> MadeBlockContextEqualsTestCases()
	{
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			null,
			false,
			"null never equals",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Pung("111z".ToTiles()), "22s".ToTiles()),
			false,
			"of different made block types",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("234s".ToTiles()), "22s".ToTiles()),
			false,
			"of different made blocks of the same type",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			true,
			"of identical contexts",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "456s".ToTiles()),
			false,
			"of different remaining tile counts",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "46s".ToTiles()),
			false,
			"of different remaining tiles",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "54s".ToTiles()),
			true,
			"of identical contexts with different remaining tile orders",
		];
	}
	private static IEnumerable<object[]> CompareMadeBlockContextsTestCases()
	{
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			null,
			1,
			"to null",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			0,
			"identical contexts",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Pung("111z".ToTiles()), "45s".ToTiles()),
			-1,
			"contexts with different made block types",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("234s".ToTiles()), "45s".ToTiles()),
			-1,
			"earlier made block to later made block",
		];
		yield return
		[
			new MadeBlockContext(new Chow("234s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			1,
			"later made block to earlier made block",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "456s".ToTiles()),
			-1,
			"shorter remaining tiles to longer remaining tiles",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "456s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			1,
			"longer remaining tiles to shorter remaining tiles",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "56s".ToTiles()),
			-1,
			"earlier remaining tiles to later remainingTiles",
		];
		yield return
		[
			new MadeBlockContext(new Chow("123s".ToTiles()), "56s".ToTiles()),
			new MadeBlockContext(new Chow("123s".ToTiles()), "45s".ToTiles()),
			1,
			"later remaining tiles to earlier remainingTiles",
		];
	}

	private static IEnumerable<object[]> CompareBlockSetsTestCases()
	{
		yield return
		[
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			null,
			1,
			"to null"
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
			]),
			1,
			"a longer set to a shorter set"
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
			]),
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			-1,
			"a shorter set to a longer set"
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("222p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			-1,
			"an earlier set to a later set"
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			new BlockSet(
			[
				new Pung("222p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			1,
			"a later set to an earlier set"
		];
		yield return
		[
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			new BlockSet(
			[
				new Pung("333p".ToTiles()),
				new PairWait("05s".ToTiles()),
				new Orphan("2z".ToTile())
			]),
			0,
			"equal sets"
		];
	}

	private static IEnumerable<object[]> GetPossibleBlockSetsTestCases()
	{
		yield return
		[
			"111m666z",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("111m".ToTiles()),
					new Pung("666z".ToTiles()),
				]),
			},
			"all pung melds",
		];
		yield return
		[
			"111m66z",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("111m".ToTiles()),
					new PairWait("66z".ToTiles()),
				]),
			},
			"a pung meld and a pair wait",
		];
		yield return
		[
			"111m0s66z",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("111m".ToTiles()),
					new PairWait("66z".ToTiles()),
					new Orphan("0s".ToTile()),
				])
			},
			"a pung meld, a pair wait, and an orphan",
		];
		yield return
		[
			"3445s",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("345s".ToTiles()),
					new Orphan("4s".ToTile()),
				]),
			},
			"a nakabukure wait",
		];
		yield return
		[
			"2345p",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("234p".ToTiles()),
					new Orphan("5p".ToTile()),
				]),
				new(
				[
					new Chow("345p".ToTiles()),
					new Orphan("2p".ToTile()),
				]),
			},
			"a nobetan wait",
		];
		yield return
		[
			"23456m",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("234m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
				]),
				new(
				[
					new Chow("345m".ToTiles()),
					new Orphan("2m".ToTile()),
					new Orphan("6m".ToTile()),
				]),
				new(
				[
					new Chow("456m".ToTiles()),
					new Ryanmen("23m".ToTiles()),
				]),
			},
			"a standard sanmenchan wait",
		];
		yield return
		[
			"2345678s",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("234s".ToTiles()),
					new Chow("567s".ToTiles()),
					new Orphan("8s".ToTile()),
				]),
				new(
				[
					new Chow("234s".ToTiles()),
					new Chow("678s".ToTiles()),
					new Orphan("5s".ToTile()),
				]),
				new(
				[
					new Chow("345s".ToTiles()),
					new Chow("678s".ToTiles()),
					new Orphan("2s".ToTile()),
				]),
				new(
				[
					new Chow("456s".ToTiles()),
					new Ryanmen("23s".ToTiles()),
					new Ryanmen("78s".ToTiles()),
				]),
			},
			"a sanmentan wait",
		];
		yield return
		[
			"44556677p11z",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("456p".ToTiles()),
					new Chow("456p".ToTiles()),
					new PairWait("77p".ToTiles()),
					new PairWait("11z".ToTiles()),
				]),
				new(
				[
					new Chow("456p".ToTiles()),
					new Chow("567p".ToTiles()),
					new Orphan("4p".ToTile()),
					new Orphan("7p".ToTile()),
					new PairWait("11z".ToTiles()),
				]),
				new(
				[
					new Chow("567p".ToTiles()),
					new Chow("568p".ToTiles()),
					new PairWait("44p".ToTiles()),
					new PairWait("11z".ToTiles()),
				]),
			},
			"a sanmen shanpon wait",
		];
		yield return
		[
			"45666s44z",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("456s".ToTiles()),
					new PairWait("66s".ToTiles()),
					new PairWait("44z".ToTiles()),
				]),
				new(
				[
					new Pung("666s".ToTiles()),
					new Ryanmen("45s".ToTiles()),
					new PairWait("44z".ToTiles()),
				]),
			},
			"an entotsu wait",
		];
		yield return
		[
			"6788m",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("678m".ToTiles()),
					new Orphan("8m".ToTile()),
				]),
			},
			"an aryanmen wait",
		];
		// TODO: When you bring ryantan over to tenpai waits, also bring its cousins pentan and kantan. The difference
		// is just wait you would end up with if this was the tenpai wait: ryanmen, penchan, or kanchan.
		// TODO: Also look at kantankan, which is similar, but doesn't show its beauty with the meld-hungry algorithm
		// I'm currently using.
		yield return
		[
			"4555p",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("555p".ToTiles()),
					new Orphan("4p".ToTile()),
				]),
			},
			"a ryantan wait",
		];
		yield return
		[
			"5566778899m",
			new List<BlockSet>()
			{
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("567m".ToTiles()),
					new PairWait("88m".ToTiles()),
					new PairWait("99m".ToTiles()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("567m".ToTiles()),
					new Penchan("89m".ToTiles()),
					new Penchan("89m".ToTiles()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("678m".ToTiles()),
					new Penchan("89m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("9m".ToTile()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("678m".ToTiles()),
					new PairWait("99m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("8m".ToTile()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("678m".ToTiles()),
					new PairWait("99m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("8m".ToTile()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("789m".ToTiles()),
					new Penchan("89m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("789m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("9m".ToTile()),
				]),
				new(
				[
					new Chow("678m".ToTiles()),
					new Chow("678m".ToTiles()),
					new PairWait("99m".ToTiles()),
					new PairWait("55m".ToTiles()),
				]),
				new(
				[
					new Chow("678m".ToTiles()),
					new Chow("789m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("9m".ToTile()),
				]),
				new(
				[
					new Chow("678m".ToTiles()),
					new Chow("789m".ToTiles()),
					new PairWait("55m".ToTiles()),
					new Orphan("6m".ToTile()),
					new Orphan("9m".ToTile()),
				]),
				new(
				[
					new Chow("789m".ToTiles()),
					new Chow("789m".ToTiles()),
					new PairWait("55m".ToTiles()),
					new PairWait("66m".ToTiles()),
				]),
				new(
				[
					new Chow("789m".ToTiles()),
					new Chow("789m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
				]),
			},
			"a goren toitsu wait",
		];
		yield return
		[
			"6667888p",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("666p".ToTiles()),
					new Pung("888p".ToTiles()),
					new Orphan("7p".ToTile()),
				]),
				new(
				[
					new Chow("678p".ToTiles()),
					new PairWait("66p".ToTiles()),
					new PairWait("88p".ToTiles()),
				]),
				new(
				[
					new Chow("678p".ToTiles()),
					new Kanchan("68p".ToTiles()),
					new Kanchan("68p".ToTiles()),
				]),
			},
			"a tatsumaki wait",
		];
		// TODO: This hand kind of reveals the limits of the meld-hungry algorithm, because one of the tenpai waits
		// would downgrade a pung of 2s down to a pair wait of 2s and a ryanmen of 23s (for example). This is probably
		// not the first case, but it's one where I think it might be a bit more important to find those block sets
		// earlier rather than later. My guess is that we'll have to extend the GetPossibleBlockSets function to take
		// in a flags enum that determines what it will search for. Alternatively, I could go with the upgrade and
		// and downgrade idea, which I kind of like as it adds to the regular language feel I'm getting here.
		yield return
		[
			"2223456777s",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("222s".ToTiles()),
					new Chow("345s".ToTiles()),
					new Pung("777s".ToTiles()),
					new Orphan("6s".ToTile()),
				]),
				new(
				[
					new Pung("222s".ToTiles()),
					new Chow("456s".ToTiles()),
					new Pung("777s".ToTiles()),
					new Orphan("3s".ToTile()),
				]),
				new(
				[
					new Chow("234s".ToTiles()),
					new Chow("567s".ToTiles()),
					new PairWait("22s".ToTiles()),
					new PairWait("77s".ToTiles()),
				]),
				new(
				[
					new Chow("234s".ToTiles()),
					new Pung("777s".ToTiles()),
					new Ryanmen("56s".ToTiles()),
					new PairWait("22s".ToTiles()),
				]),
				new(
				[
					new Pung("222s".ToTiles()),
					new Chow("567s".ToTiles()),
					new Ryanmen("34s".ToTiles()),
					new PairWait("77s".ToTiles()),
				]),
			},
			"a happoubijin wait",
		];
		yield return
		[
			"2223456677778m",
			new List<BlockSet>()
			{
				new(
				[
					new Pung("222m".ToTiles()),
					new Chow("345m".ToTiles()),
					new Chow("678m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Orphan("6m".ToTile()),
				]),
				new(
				[
					new Chow("234m".ToTiles()),
					new Chow("567m".ToTiles()),
					new Chow("678m".ToTiles()),
					new PairWait("22m".ToTiles()),
					new PairWait("77m".ToTiles()),
				]),
				new(
				[
					new Pung("222m".ToTiles()),
					new Chow("456m".ToTiles()),
					new Chow("678m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Orphan("3m".ToTile()),
				]),
				new(
				[
					new Pung("222m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Chow("567m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new Ryanmen("34m".ToTiles()),
				]),
				new(
				[
					new Pung("222m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Chow("567m".ToTiles()),
					new Kanchan("46m".ToTiles()),
					new Orphan("3m".ToTile()),
					new Orphan("8m".ToTile()),
				]),
				// These are the ones I missed :(
				new(
				[
					new Chow("234m".ToTiles()),
					new Chow("567m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new PairWait("22m".ToTiles()),
				]),
				new(
				[
					new Chow("234m".ToTiles()),
					new Chow("678m".ToTiles()),
					new Pung("777m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
					new PairWait("22m".ToTiles()),
				]),
				new(
				[
					new Chow("234m".ToTiles()),
					new Kong("7777m".ToTiles()),
					new Ryanmen("56m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new PairWait("22m".ToTiles()),
				]),
				new(
				[
					new Chow("345m".ToTiles()),
					new Pung("222m".ToTiles()),
					new Kong("7777m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new Orphan("6m".ToTile()),
				]),
				new(
				[
					new Chow("345m".ToTiles()),
					new Pung("222m".ToTiles()),
					new Kong("7777m".ToTiles()),
					new PairWait("66m".ToTiles()),
					new Orphan("8m".ToTile()),
				]),
				new(
				[
					new Chow("456m".ToTiles()),
					new Pung("222m".ToTiles()),
					new Kong("7777m".ToTiles()),
					new Kanchan("68m".ToTiles()),
					new Orphan("3m".ToTile()),
				]),
				new(
				[
					new Chow("567m".ToTiles()),
					new Chow("678m".ToTiles()),
					new Pung("222m".ToTiles()),
					new Ryanmen("34m".ToTiles()),
					new PairWait("77m".ToTiles()),
				]),
				new(
				[
					new Chow("234m".ToTiles()),
					new Kong("7777m".ToTiles()),
					new PairWait("22m".ToTiles()),
					new PairWait("66m".ToTiles()),
					new Orphan("5m".ToTile()),
					new Orphan("8m".ToTile()),
				]),
			},
			"a paaren poutou wait",
		];
	}
}