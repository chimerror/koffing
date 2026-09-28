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
		int expectedLiveWallEnd,
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
				PrefixInfo($"Checking that the number of {rank} of {suit} in the wall is {tileCountExpected}...");
				var tileCountActual = createdWallTiles.Count(t => t.Rank == rank && t.Suit == suit);
				AssertThat(tileCountActual).IsEqual(tileCountExpected);
			}

			var redFivesExpected = hasRedFives ? 1 : 0;
			PrefixInfo($"Checking that there are {redFivesExpected} red fives of {suit} in the wall...");
		}

		PrefixInfo($"Checking that start of live wall and next live tile is index {expectedLiveWallStart}...");
		AssertThat(createdWall.LiveWallStartIndex).IsEqual(expectedLiveWallStart);
		AssertThat(createdWall.NextLiveTileIndex).IsEqual(expectedLiveWallStart);

		PrefixInfo($"Checking that end of live wall is index {expectedLiveWallStart}...");
		AssertThat(createdWall.LiveWallEndIndex).IsEqual(expectedLiveWallEnd);

		PrefixInfo($"Checking that next replacement Tile is index {expectedNextReplacementTile}...");
		AssertThat(createdWall.NextReplacementTileIndex).IsEqual(expectedNextReplacementTile);

		PrefixInfo($"Checking that last revealed dora indicator is index {expectedLastRevealedDoraIndicator}...");
		AssertThat(createdWall.LastRevealedDoraIndicatorIndex).IsEqual(expectedLastRevealedDoraIndicator);

		PrefixInfo($"Checking that next dora indicator is index {expectedNextDoraIndicator}...");
		AssertThat(createdWall.NextDoraIndicatorIndex).IsEqual(expectedNextDoraIndicator);

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
			23, // Live Wall End
			36, // Next Replacement Tile
			32, // Last Revealed Dora Indicator
			30, // Next Dora Indicator
			"9p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			3, // Dice Roll
			74, // Live Wall Start, Next Live Tile
			59, // Live Wall End
			72, // Next Replacement Tile
			68, // Last Revealed Dora Indicator
			66, // Next Dora Indicator
			"2m".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			4, // Dice Roll
			110, // Live Wall Start, Next Live Tile
			95, // Live Wall End
			108, // Next Replacement Tile
			104, // Last Revealed Dora Indicator
			102, // Next Dora Indicator
			"4s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Two,
			true, // Red Fives?
			5, // Dice Roll
			10, // Live Wall Start, Next Live Tile
			131, // Live Wall End
			8, // Next Replacement Tile
			4, // Last Revealed Dora Indicator
			2, // Next Dora Indicator
			"3m".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			6, // Dice Roll
			46, // Live Wall Start, Next Live Tile
			31, // Live Wall End
			44, // Next Replacement Tile
			40, // Last Revealed Dora Indicator
			38, // Next Dora Indicator
			"8p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			7, // Dice Roll
			82, // Live Wall Start, Next Live Tile
			67, // Live Wall End
			80, // Next Replacement Tile
			76, // Last Revealed Dora Indicator
			74, // Next Dora Indicator
			"8p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			8, // Dice Roll
			118, // Live Wall Start, Next Live Tile
			103, // Live Wall End
			116, // Next Replacement Tile
			112, // Last Revealed Dora Indicator
			110, // Next Dora Indicator
			"2p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Two,
			true, // Red Fives?
			9, // Dice Roll
			18, // Live Wall Start, Next Live Tile
			3, // Live Wall End
			16, // Next Replacement Tile
			12, // Last Revealed Dora Indicator
			10, // Next Dora Indicator
			"1s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			10, // Dice Roll
			54, // Live Wall Start, Next Live Tile
			39, // Live Wall End
			52, // Next Replacement Tile
			48, // Last Revealed Dora Indicator
			46, // Next Dora Indicator
			"6p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Four,
			true, // Red Fives?
			11, // Dice Roll
			90, // Live Wall Start, Next Live Tile
			75, // Live Wall End
			88, // Next Replacement Tile
			84, // Last Revealed Dora Indicator
			82, // Next Dora Indicator
			"5s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Two,
			false, // Red Fives?
			12, // Dice Roll
			126, // Live Wall Start, Next Live Tile
			111, // Live Wall End
			124, // Next Replacement Tile
			120, // Last Revealed Dora Indicator
			118, // Next Dora Indicator
			"4m".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			2, // Dice Roll
			40, // Live Wall Start, Next Live Tile
			19, // Live Wall End
			38, // Next Replacement Tile
			30, // Last Revealed Dora Indicator
			28, // Next Dora Indicator
			"7z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			3, // Dice Roll
			78, // Live Wall Start, Next Live Tile
			57, // Live Wall End
			76, // Next Replacement Tile
			68, // Last Revealed Dora Indicator
			66, // Next Dora Indicator
			"1p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			4, // Dice Roll
			8, // Live Wall Start, Next Live Tile
			95, // Live Wall End
			6, // Next Replacement Tile
			106, // Last Revealed Dora Indicator
			104, // Next Dora Indicator
			"1m".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			5, // Dice Roll
			46, // Live Wall Start, Next Live Tile
			25, // Live Wall End
			44, // Next Replacement Tile
			36, // Last Revealed Dora Indicator
			34, // Next Dora Indicator
			"2z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			6, // Dice Roll
			84, // Live Wall Start, Next Live Tile
			63, // Live Wall End
			82, // Next Replacement Tile
			74, // Last Revealed Dora Indicator
			72, // Next Dora Indicator
			"4z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			7, // Dice Roll
			14, // Live Wall Start, Next Live Tile
			101, // Live Wall End
			12, // Next Replacement Tile
			4, // Last Revealed Dora Indicator
			2, // Next Dora Indicator
			"9m".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			8, // Dice Roll
			52, // Live Wall Start, Next Live Tile
			31, // Live Wall End
			50, // Next Replacement Tile
			42, // Last Revealed Dora Indicator
			40, // Next Dora Indicator
			"6s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			9, // Dice Roll
			90, // Live Wall Start, Next Live Tile
			69, // Live Wall End
			88, // Next Replacement Tile
			80, // Last Revealed Dora Indicator
			78, // Next Dora Indicator
			"3p".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			10, // Dice Roll
			20, // Live Wall Start, Next Live Tile
			107, // Live Wall End
			18, // Next Replacement Tile
			10, // Last Revealed Dora Indicator
			8, // Next Dora Indicator
			"4z".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			false, // Red Fives?
			11, // Dice Roll
			58, // Live Wall Start, Next Live Tile
			37, // Live Wall End
			56, // Next Replacement Tile
			48, // Last Revealed Dora Indicator
			46, // Next Dora Indicator
			"1s".ToTile(), // Expected Revealed Dora
		];
		yield return
		[
			PlayerCount.Three,
			true, // Red Fives?
			12, // Dice Roll
			96, // Live Wall Start, Next Live Tile
			75, // Live Wall End
			94, // Next Replacement Tile
			86, // Last Revealed Dora Indicator
			84, // Next Dora Indicator
			"3z".ToTile(), // Expected Revealed Dora
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