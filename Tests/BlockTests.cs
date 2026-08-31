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

		PrefixInfo($"Checking that block A {blockA} does not equal object B {objectB} because");
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

	private static IEnumerable<object[]> BlockEqualsEdgeTestCases()
	{
		yield return [new Chow("123s".ToAutoFreeTiles()), null, "null should not equal"];
		yield return [new Chow("123s".ToAutoFreeTiles()), "123s", "different types should not equal"];
	}

	private static IEnumerable<object[]> BlockEqualsTestCases()
	{
		// A lot of these may be "incorrect" blocks as far as their actual tiles, but that is not part of the guarantee
		// for the Block Classes. Instead, you should just make sure to use the GetPossible methods to generate them,
		// which will _only_ generate correct blocks. I guess this could bite me in the tail if I do a bad job, but
		// I have already made working tests of the GetPossibleForTile methods.

		yield return
		[
			new Pung("444p".ToAutoFreeTiles()),
			null,
			false,
			"null should not equal",
		];
		yield return
		[
			new Pung("444p".ToAutoFreeTiles()),
			new Chow("444p".ToAutoFreeTiles()),
			false,
			"type should matter",
		];
		yield return
		[
			new Pung("444z".ToAutoFreeTiles()),
			new Pung("44z".ToAutoFreeTiles()),
			false,
			"tile counts should matter",
		];
		yield return
		[
			new Chow("456s".ToAutoFreeTiles()),
			new Chow("456s".ToAutoFreeTiles()),
			true,
			"they have the same type and tiles",
		];
		yield return
		[
			new Chow("456m".ToAutoFreeTiles()),
			new Chow("546m".ToAutoFreeTiles()),
			true,
			"tile order should not matter",
		];
		yield return
		[
			new Chow("456p".ToAutoFreeTiles()),
			new Chow("234p".ToAutoFreeTiles()),
			false,
			"they are the same type but not the same tiles",
		];
	}
}