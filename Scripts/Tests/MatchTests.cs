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
		int expectedNextDoraIndicator,
		Tile expectedRevealedDoraIndicatorTile
	)
	{
		LoggingPrefix = nameof(WallIsCreatedCorrectly);

		SystemRandomNumberGenerator rng = new()
		{
			Seed = 13
		};
		var createdWall = new Wall(rng, playerCount, hasRedFives, breakDiceRoll);
		var createdWallTiles = createdWall.Tiles.ToList();

		var expectedTileCount = playerCount == PlayerCount.Three ? 108 : 136;
		PrefixInfo($"Checking tile count is {expectedTileCount}");
		AssertThat(createdWallTiles.Count).IsEqual(expectedTileCount);

		var redFivesOutcome = hasRedFives ? "does" : "does NOT";
		var redFiveCountExpected = hasRedFives ? 3 : 0;
		if (hasRedFives && playerCount == PlayerCount.Three)
		{
			redFiveCountExpected--;
		}
		PrefixInfo($"Checking that created wall {redFivesOutcome} has the right number of red fives...");
		AssertThat(createdWallTiles.Count(t => t.Rank == 0)).IsEqual(redFiveCountExpected);

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 1; rank <= 9; rank++)
			{
				if (suit == Suit.Zi && rank > 8)
				{
					// No need to test impossible honors more than once.
					break;
				}

				var tileCountExpected = Tile.IsValidTile(suit, rank) ? 4 : 0;
				if (playerCount == PlayerCount.Three && suit == Suit.Man && rank != 1 && rank != 9)
				{
					tileCountExpected = 0;
				}
				else if (suit != Suit.Zi && hasRedFives && rank == 5)
				{
					tileCountExpected--;
				}
				PrefixInfo($"Checking that there are {tileCountExpected} {rank} of {suit} in the wall...");
				var tileCountActual = createdWallTiles.Count(t => t.Rank == rank && t.Suit == suit);
				AssertThat(tileCountActual).IsEqual(tileCountExpected);
			}

			var redFivesExpected = hasRedFives ? 1 : 0;
			PrefixInfo($"Checking that there are {redFivesExpected} red fives of {suit} in the wall...");
		}

		// TODO: I learned that IsEqual makes this work the opposite way from the Array assertions, so these parameters
		// should be swapped.
		PrefixInfo($"Checking that start of live wall and next live tile is index {expectedLiveWallStart}...");
		AssertThat(expectedLiveWallStart).IsEqual(createdWall.LiveWallStartIndex);
		AssertThat(expectedLiveWallStart).IsEqual(createdWall.NextLiveTileIndex);

		PrefixInfo($"Checking that next replacement Tile is index {expectedNextReplacementTile}...");
		AssertThat(expectedNextReplacementTile).IsEqual(createdWall.NextReplacementTileIndex);

		PrefixInfo($"Checking that last revealed dora indicator is index {expectedLastRevealedDoraIndicator}...");
		AssertThat(expectedLastRevealedDoraIndicator).IsEqual(createdWall.LastRevealedDoraIndicatorIndex);

		PrefixInfo($"Checking that next dora indicator is index {expectedNextDoraIndicator}...");
		AssertThat(expectedNextDoraIndicator).IsEqual(createdWall.NextDoraIndicatorIndex);

		PrefixInfo("Checking there is only one face up tile...");
		AssertThat(1).IsEqual(createdWallTiles.Count(t => t.FaceUp));

		var revealedDoraIndicator = createdWallTiles[createdWall.LastRevealedDoraIndicatorIndex];
		PrefixInfo("Checking that the revealed Dora is face up...");
		AssertThat(revealedDoraIndicator.FaceUp).IsTrue();
		PrefixInfo($"Checking that the revealed Dora is {expectedRevealedDoraIndicatorTile}...");
		AssertThat(revealedDoraIndicator).IsEqual(expectedRevealedDoraIndicatorTile);
	}

	[TestCase]
	[DataPoint(nameof(MatchCreationTestCases))]
	public static void MatchIsCreatedCorrectly(
		PlayerCount playerCount,
		bool hasRedFives,
		int numberOfRounds,
		int expectedStartingEastIndex)
	{
		LoggingPrefix = nameof(MatchIsCreatedCorrectly);

		SystemRandomNumberGenerator rng = new()
		{
			Seed = 13
		};
		var createdMatch = new Match(rng, playerCount, hasRedFives, numberOfRounds);
		PrefixInfo($"Checking that created match has the right player count enum {playerCount}");
		AssertThat(playerCount).IsEqual(createdMatch.PlayerCount);
		PrefixInfo($"Checking that created match has the right red five setting {hasRedFives}");
		AssertThat(hasRedFives).IsEqual(createdMatch.HasRedFives);
		PrefixInfo($"Checking that created match has the right number of rounds {numberOfRounds}");
		AssertThat(numberOfRounds).IsEqual(createdMatch.NumberOfRounds);

		var actualPlayers = createdMatch.Players.ToList();
		PrefixInfo("Checking that there are the right number of players...");
		AssertThat((int)playerCount).IsEqual(actualPlayers.Count);
		PrefixInfo("Checking that the players have the correct starting scores...");
		AssertThat(actualPlayers.All(p => p.Points == (playerCount == PlayerCount.Three ? 30000 : 25000)));

		PrefixInfo($"Checking that the correct player (index {expectedStartingEastIndex}) was chosen as East...");
		AssertThat(createdMatch.CurrentEastIndex).IsEqual(expectedStartingEastIndex);
		AssertObject(createdMatch.CurrentEastPlayer).IsSame(actualPlayers[expectedStartingEastIndex]);
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
			"6m".ToTile(), // Expected Revealed Dora
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
			"4m".ToTile(), // Expected Revealed Dora
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
			"5m".ToTile(), // Expected Revealed Dora
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
			"9p".ToTile(), // Expected Revealed Dora
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
			"9s".ToTile(), // Expected Revealed Dora
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
			"9p".ToTile(), // Expected Revealed Dora
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
			"8m".ToTile(), // Expected Revealed Dora
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
			"2s".ToTile(), // Expected Revealed Dora
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
			"9s".ToTile(), // Expected Revealed Dora
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
			"6m".ToTile(), // Expected Revealed Dora
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
			"8s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			2, // Dice Roll
			40, // Live Wall Start, Next Live Tile
			20, // Next Replacement Tile
			28, // Last Revealed Dora Indicator
			30, // Next Dora Indicator
			"3p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			3, // Dice Roll
			78, // Live Wall Start, Next Live Tile
			58, // Next Replacement Tile
			66, // Last Revealed Dora Indicator
			68, // Next Dora Indicator
			"2p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			4, // Dice Roll
			8, // Live Wall Start, Next Live Tile
			96, // Next Replacement Tile
			104, // Last Revealed Dora Indicator
			106, // Next Dora Indicator
			"4p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			5, // Dice Roll
			46, // Live Wall Start, Next Live Tile
			26, // Next Replacement Tile
			34, // Last Revealed Dora Indicator
			36, // Next Dora Indicator
			"4p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			6, // Dice Roll
			84, // Live Wall Start, Next Live Tile
			64, // Next Replacement Tile
			72, // Last Revealed Dora Indicator
			74, // Next Dora Indicator
			"4s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			7, // Dice Roll
			14, // Live Wall Start, Next Live Tile
			102, // Next Replacement Tile
			2, // Last Revealed Dora Indicator
			4, // Next Dora Indicator
			"4z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			8, // Dice Roll
			52, // Live Wall Start, Next Live Tile
			32, // Next Replacement Tile
			40, // Last Revealed Dora Indicator
			42, // Next Dora Indicator
			"2s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			9, // Dice Roll
			90, // Live Wall Start, Next Live Tile
			70, // Next Replacement Tile
			78, // Last Revealed Dora Indicator
			80, // Next Dora Indicator
			"2z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			10, // Dice Roll
			20, // Live Wall Start, Next Live Tile
			0, // Next Replacement Tile
			8, // Last Revealed Dora Indicator
			10, // Next Dora Indicator
			"5s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			11, // Dice Roll
			58, // Live Wall Start, Next Live Tile
			38, // Next Replacement Tile
			46, // Last Revealed Dora Indicator
			48, // Next Dora Indicator
			"2z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			12, // Dice Roll
			96, // Live Wall Start, Next Live Tile
			76, // Next Replacement Tile
			84, // Last Revealed Dora Indicator
			86, // Next Dora Indicator
			"8s".ToTile(), // Expected Revealed Dora
		];
	}

	private static IEnumerable<object[]> MatchCreationTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			true,
			2,
			0,
		];
		yield return
		[
			PlayerCount.Four,
			false,
			2,
			0,
		];
		yield return
		[
			PlayerCount.Four,
			true,
			1,
			0,
		];
		yield return
		[
			PlayerCount.Four,
			false,
			1,
			0,
		];
		yield return
		[
			PlayerCount.Three,
			true,
			2,
			1,
		];
		yield return
		[
			PlayerCount.Three,
			false,
			2,
			1,
		];
		yield return
		[
			PlayerCount.Three,
			true,
			1,
			1,
		];
		yield return
		[
			PlayerCount.Three,
			false,
			1,
			1,
		];
		yield return
		[
			PlayerCount.Two,
			true,
			2,
			0,
		];
		yield return
		[
			PlayerCount.Two,
			false,
			2,
			0,
		];
		yield return
		[
			PlayerCount.Two,
			true,
			1,
			0,
		];
		yield return
		[
			PlayerCount.Two,
			false,
			1,
			0,
		];
	}
}