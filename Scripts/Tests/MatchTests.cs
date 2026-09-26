using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class MatchTests
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

	// TODO: In a different test, check that we have the right tiles at the right locations based on a seed.
	[TestCase]
	[DataPoint(nameof(WallCreationTestCases))]
	public static void WallIsCreatedCorrectly(
		PlayerCount playerCount,
		bool hasRedFives,
		int breakDiceRoll,
		int expectedLiveWallStart,
		int expectedNextReplacementTile,
		int expectedLastRevealedDoraIndicator,
		int expectedNextDoraIndicator
	)
	{
		LoggingPrefix = nameof(WallIsCreatedCorrectly);

		SystemRandomNumberGenerator rng = new();
		var createdWall = new Wall(rng, playerCount, hasRedFives, breakDiceRoll);

		var expectedTileCount = playerCount == PlayerCount.Three ? 108 : 136;
		PrefixInfo($"Checking tile count is {expectedTileCount}");
		AssertThat(createdWall.Tiles.Count()).IsEqual(expectedTileCount);

		var redFivesOutcome = hasRedFives ? "does" : "does NOT";
		var redFiveCountExpected = hasRedFives ? 3 : 0;
		if (hasRedFives && playerCount == PlayerCount.Three)
		{
			redFiveCountExpected--;
		}
		PrefixInfo($"Checking that created wall {redFivesOutcome} has the right number of red fives...");
		AssertThat(createdWall.Tiles.Count(t => t.Rank == 0)).IsEqual(redFiveCountExpected);

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 1; rank <= 9; rank++)
			{
				if (suit == Suit.Zi && rank > 8)
				{
					// No need to test impossible honors more than once.
					break;
				}

				var tilesExpected = Tile.IsValidTile(suit, rank) ? 4 : 0;
				if (suit != Suit.Zi && hasRedFives && rank == 5)
				{
					tilesExpected--;
				}
				else if (playerCount == PlayerCount.Three && suit == Suit.Man && rank != 1 && rank != 9)
				{
					tilesExpected = 0;
				}
				PrefixInfo($"Checking that there are {tilesExpected} {rank} of {suit} in the wall...");
				AssertThat(createdWall.Tiles.Count(t => t.Suit == suit && t.Rank == rank));
			}

			var redFivesExpected = hasRedFives ? 1 : 0;
			PrefixInfo($"Checking that there are {redFivesExpected} red fives of {suit} in the wall...");
		}

		PrefixInfo($"Checking that start of live wall and next live tile is index {expectedLiveWallStart}...");
		AssertThat(expectedLiveWallStart).IsEqual(createdWall.LiveWallStartIndex);
		AssertThat(expectedLiveWallStart).IsEqual(createdWall.NextLiveTileIndex);

		PrefixInfo($"Checking that next replacement Tile is index {expectedNextReplacementTile}...");
		AssertThat(expectedNextReplacementTile).IsEqual(createdWall.NextReplacementTileIndex);

		PrefixInfo($"Checking that last revealed dora indicator is index {expectedLastRevealedDoraIndicator}...");
		AssertThat(expectedLastRevealedDoraIndicator).IsEqual(createdWall.LastRevealedDoraIndicatorIndex);

		PrefixInfo($"Checking that next dora indicator is index {expectedNextDoraIndicator}...");
		AssertThat(expectedNextDoraIndicator).IsEqual(createdWall.NextDoraIndicatorIndex);
	}

	private static IEnumerable<object[]> WallCreationTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			2, // Dice Roll
			38, // Live Wall Start, Next Live Tile
			24, // Next Replacement Tile
			28, // Last Revealed Dora Indicator
			30, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			3, // Dice Roll
			74, // Live Wall Start, Next Live Tile
			60, // Next Replacement Tile
			64, // Last Revealed Dora Indicator
			66, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			4, // Dice Roll
			110, // Live Wall Start, Next Live Tile
			96, // Next Replacement Tile
			100, // Last Revealed Dora Indicator
			102, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Two,
			true, // Red Fives?
			5, // Dice Roll
			10, // Live Wall Start, Next Live Tile
			132, // Next Replacement Tile
			0, // Last Revealed Dora Indicator
			2, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			6, // Dice Roll
			46, // Live Wall Start, Next Live Tile
			32, // Next Replacement Tile
			36, // Last Revealed Dora Indicator
			38, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			7, // Dice Roll
			82, // Live Wall Start, Next Live Tile
			68, // Next Replacement Tile
			72, // Last Revealed Dora Indicator
			74, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			8, // Dice Roll
			118, // Live Wall Start, Next Live Tile
			104, // Next Replacement Tile
			108, // Last Revealed Dora Indicator
			110, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Two,
			true, // Red Fives?
			9, // Dice Roll
			18, // Live Wall Start, Next Live Tile
			4, // Next Replacement Tile
			8, // Last Revealed Dora Indicator
			10, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			10, // Dice Roll
			54, // Live Wall Start, Next Live Tile
			40, // Next Replacement Tile
			44, // Last Revealed Dora Indicator
			46, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			11, // Dice Roll
			90, // Live Wall Start, Next Live Tile
			76, // Next Replacement Tile
			80, // Last Revealed Dora Indicator
			82, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			12, // Dice Roll
			126, // Live Wall Start, Next Live Tile
			112, // Next Replacement Tile
			116, // Last Revealed Dora Indicator
			118, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			2, // Dice Roll
			40, // Live Wall Start, Next Live Tile
			26, // Next Replacement Tile
			30, // Last Revealed Dora Indicator
			32, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			3, // Dice Roll
			78, // Live Wall Start, Next Live Tile
			64, // Next Replacement Tile
			68, // Last Revealed Dora Indicator
			70, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			4, // Dice Roll
			8, // Live Wall Start, Next Live Tile
			102, // Next Replacement Tile
			106, // Last Revealed Dora Indicator
			0, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			5, // Dice Roll
			46, // Live Wall Start, Next Live Tile
			32, // Next Replacement Tile
			36, // Last Revealed Dora Indicator
			38, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			6, // Dice Roll
			84, // Live Wall Start, Next Live Tile
			70, // Next Replacement Tile
			74, // Last Revealed Dora Indicator
			76, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			7, // Dice Roll
			14, // Live Wall Start, Next Live Tile
			0, // Next Replacement Tile
			4, // Last Revealed Dora Indicator
			6, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			8, // Dice Roll
			52, // Live Wall Start, Next Live Tile
			38, // Next Replacement Tile
			42, // Last Revealed Dora Indicator
			44, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			9, // Dice Roll
			90, // Live Wall Start, Next Live Tile
			76, // Next Replacement Tile
			80, // Last Revealed Dora Indicator
			82, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			10, // Dice Roll
			20, // Live Wall Start, Next Live Tile
			6, // Next Replacement Tile
			10, // Last Revealed Dora Indicator
			12, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			11, // Dice Roll
			58, // Live Wall Start, Next Live Tile
			44, // Next Replacement Tile
			48, // Last Revealed Dora Indicator
			50, // Next Dora Indicator
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			12, // Dice Roll
			96, // Live Wall Start, Next Live Tile
			82, // Next Replacement Tile
			86, // Last Revealed Dora Indicator
			88, // Next Dora Indicator
		];
	}
}