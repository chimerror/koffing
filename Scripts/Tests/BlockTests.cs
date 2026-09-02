using System.Collections.Generic;
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
	[DataPoint(nameof(BlockEqualsEdgeTestCases))]
	public static void BlockEqualsEdgeCasesAreCorrect(Block blockA, object objectB, string because)
	{
		LoggingPrefix = nameof(BlockEqualsEdgeCasesAreCorrect);

		PrefixInfo($"Checking that block A {blockA} does not equal object B {objectB} because {because}");
		AssertThat(blockA.Equals(objectB)).IsFalse();
	}

	[TestCase]
	[DataPoint(nameof(BlockEqualsTestCases))]
	public static void BlockEqualsIsCorrect(Block blockA, Block blockB, bool expectedResult, string because)
	{
		LoggingPrefix = nameof(BlockEqualsIsCorrect);

		var outcomeString = expectedResult ? "does equal" : "does NOT equal";
		PrefixInfo($"Checking that block A {blockA} {outcomeString} block B {blockB} because {because}");
		AssertThat(blockA.Equals(blockB)).IsEqual(expectedResult);
		if (blockB != null)
		{
			PrefixInfo($"Checking that block B {blockB} {outcomeString} block A {blockA} because {because}");
			AssertThat(blockB.Equals(blockA)).IsEqual(expectedResult);
		}
	}

	[TestCase]
	[DataPoint(nameof(GetBlockHashCodeTestCases))]
	public static void GetBlockHashCodeIsCorrect(Block block, int expectedHashCode)
	{
		LoggingPrefix = nameof(GetBlockHashCodeIsCorrect);

		PrefixInfo($"Checking that block of type {block.GetType()} has hash code {expectedHashCode}");
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

		PrefixInfo($"Checking that made block context A {contextA} does not equal object B {objectB} because {because}");
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
		PrefixInfo($"Checking that context A {contextA} {outcomeString} context B {contextB} because {because}");
		AssertThat(contextA.Equals(contextB)).IsEqual(expectedResult);
		if (contextB != null)
		{
			PrefixInfo($"Checking that context B {contextB} {outcomeString} context A {contextA} because {because}");
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
}