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

	private static IEnumerable<object[]> GetBlockHashCodeTestCases()
	{
		// No tiles in the blocks because that way the exponent remains 1 and we just get the basis, which is what we
		// are really testing here.
		// TODO: We do seem to have 3 orphans from this test, which is odd because we create a List<Tile>, but it never
		// as anything in it, so I don't know why it gets noticed as a Godot object.
		yield return [new Chow(), 2];
		yield return [new Pung(), 3];
		yield return [new Kong(), 5];
	}

	private static IEnumerable<object[]> CompareBlocksTestCases()
	{
		// Some of these blocks are "wrong" compared to their names, but this is for the sake of testing.
		yield return [new Chow("123s".ToAutoFreeTiles()), null, 1, "comparing to null block"];
		yield return [
			new Chow("123s".ToAutoFreeTiles()),
			new Chow("123s".ToAutoFreeTiles()),
			0,
			"comparing identical Chows",
		];
		yield return [
			new Chow("123s".ToAutoFreeTiles()),
			new Pung("111z".ToAutoFreeTiles()),
			-1,
			"comparing Chow to Pung",
		];
		yield return [
			new Pung("111z".ToAutoFreeTiles()),
			new Chow("123s".ToAutoFreeTiles()),
			1,
			"comparing Pung to Chow",
		];
		yield return [
			new Chow("123s".ToAutoFreeTiles()),
			new Kong("4444m".ToAutoFreeTiles()),
			-1,
			"comparing Chow to Kong",
		];
		yield return [
			new Kong("4444m".ToAutoFreeTiles()),
			new Chow("123s".ToAutoFreeTiles()),
			1,
			"comparing Kong to Chow",
		];
		yield return [
			new Pung("111z".ToAutoFreeTiles()),
			new Pung("111z".ToAutoFreeTiles()),
			0,
			"comparing identical Pungs",
		];
		yield return [
			new Pung("111z".ToAutoFreeTiles()),
			new Kong("4444m".ToAutoFreeTiles()),
			-1,
			"comparing Pung to Kong",
		];
		yield return [
			new Kong("4444m".ToAutoFreeTiles()),
			new Pung("111z".ToAutoFreeTiles()),
			1,
			"comparing Kong to Pung",
		];
		yield return [
			new Kong("4444m".ToAutoFreeTiles()),
			new Kong("4444m".ToAutoFreeTiles()),
			0,
			"comparing identical Kongs",
		];
		yield return [
			new Chow("123s".ToAutoFreeTiles()),
			new Chow("1234s".ToAutoFreeTiles()),
			-1,
			"comparing shorter to longer of the same type",
		];
		yield return [
			new Chow("1234s".ToAutoFreeTiles()),
			new Chow("123s".ToAutoFreeTiles()),
			1,
			"comparing longer to shorter of the same type",
		];
		yield return [
			new Chow("123s".ToAutoFreeTiles()),
			new Chow("234s".ToAutoFreeTiles()),
			-1,
			"comparing earlier to later of the same type",
		];
		yield return [
			new Chow("234s".ToAutoFreeTiles()),
			new Chow("123s".ToAutoFreeTiles()),
			1,
			"comparing later to earlier of the same type",
		];
	}
}