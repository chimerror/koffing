using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;

using static GdUnit4.Assertions;
using static Helpers;
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
	[DataPoint(nameof(PopLiveTileTestCases))]
	public static void PopLiveTileIsCorrect(
		PlayerCount playerCount,
		bool hasRedFives,
		IEnumerable<Tile> expectedPoppedTiles
	)
	{
		LoggingPrefix = nameof(PopLiveTileIsCorrect);

		SystemRandomNumberGenerator rng = new()
		{
			Seed = 13
		};
		var createdWall = new Wall(rng, playerCount, hasRedFives);

		PrefixInfo("Checking that CanTakeLiveTile reports that a tile can be taken...");
		AssertThat(createdWall.CanTakeLiveTile).IsTrue();

		List<Tile> actualPoppedTiles = [];
		while (createdWall.CanTakeLiveTile)
		{
			actualPoppedTiles.Add(createdWall.PopNextLiveTile());
		}

		// PrefixInfo($"DEBUG: Actual Popped Tiles: \"{actualPoppedTiles.NotationFromTiles()}\"");
		PrefixInfo("Checking that actual popped tiles matched expected popped tiles...");
		AssertArray(expectedPoppedTiles).ContainsExactly(actualPoppedTiles);

		PrefixInfo("Checking that CanTakeLiveTile reports that a tile can't be taken...");
		AssertThat(createdWall.CanTakeLiveTile).IsFalse();

		var exhaustedWallTile = createdWall.PopNextLiveTile();
		PrefixInfo("Checking that PopNextLiveTile returns null...");
		AssertObject(exhaustedWallTile).IsNull();
	}

	[TestCase]
	[DataPoint(nameof(PopReplacementTileTestCases))]
	public static void PopReplacementTileIsCorrect(
		PlayerCount playerCount,
		bool hasRedFives,
		int numberOfReplacementTilesToPop,
		IEnumerable<Tile> expectedPoppedReplacementTiles,
		IEnumerable<Tile> expectedPoppedLiveTiles)
	{
		LoggingPrefix = nameof(PopReplacementTileIsCorrect);

		SystemRandomNumberGenerator rng = new()
		{
			Seed = 13
		};
		var createdWall = new Wall(rng, playerCount, hasRedFives);

		PrefixInfo("Checking that CanTakeReplacementTile reports that a tile can be taken...");
		AssertThat(createdWall.CanTakeReplacementTile).IsTrue();

		List<Tile> actualPoppedReplacementTiles = [];
		if (numberOfReplacementTilesToPop == -1)
		{
			while(createdWall.CanTakeReplacementTile)
			{
				actualPoppedReplacementTiles.Add(createdWall.PopNextReplacementTile());
			}
		}
		else
		{
			for (int i = 0; i < numberOfReplacementTilesToPop; i++)
			{
				actualPoppedReplacementTiles.Add(createdWall.PopNextReplacementTile());
			}
		}

		// PrefixInfo($"DEBUG: Actual Popped Replacement Tiles: \"{actualPoppedReplacementTiles.NotationFromTiles()}\"");
		PrefixInfo("Checking that actual popped replacement tiles matched expected popped replacement tiles...");
		AssertArray(expectedPoppedReplacementTiles).ContainsExactly(actualPoppedReplacementTiles);

		var replacementTilesExhausted =
			numberOfReplacementTilesToPop == -1 ||
			numberOfReplacementTilesToPop == (playerCount == PlayerCount.Three ? 8 : 4);
		if (replacementTilesExhausted)
		{
			PrefixInfo("Checking that CanTakeReplacementTile reports that a tile can't be taken after exhaustion...");
			AssertThat(createdWall.CanTakeReplacementTile).IsFalse();

			var exhaustedReplacementTile = createdWall.PopNextReplacementTile();
			PrefixInfo("Checking that PopNextReplacementTile returns null...");
			AssertObject(exhaustedReplacementTile).IsNull();
		}
		else
		{
			PrefixInfo("Checking that CanTakeReplacementTile reports that a tile can still be taken after taking less than the available number of tiles...");
			AssertThat(createdWall.CanTakeReplacementTile).IsTrue();
		}

		List<Tile> actualPoppedLiveTiles = [];
		while (createdWall.CanTakeLiveTile)
		{
			actualPoppedLiveTiles.Add(createdWall.PopNextLiveTile());
		}
		PrefixInfo("Checking that the correct number of live tiles were not drawn after taking replacement tiles...");
		AssertArray(expectedPoppedLiveTiles).ContainsExactly(actualPoppedLiveTiles);
	}

	[TestCase]
	[DataPoint(nameof(RevealDoraIndicatorTestCases))]
	public static void RevealDoraIndicatorIsCorrect(
		PlayerCount playerCount,
		bool hasRedFives,
		int numberOfDoraTilesToReveal,
		IEnumerable<Tile> expectedRevealedDoraIndicators)
	{
		LoggingPrefix = nameof(RevealDoraIndicatorIsCorrect);

		SystemRandomNumberGenerator rng = new()
		{
			Seed = 13
		};
		var createdWall = new Wall(rng, playerCount, hasRedFives);

		PrefixInfo("Checking that CanRevealDoraIndicator reports that a tile can be taken...");
		AssertThat(createdWall.CanRevealDoraIndicator).IsTrue();

		var wallTiles = createdWall.Tiles.ToList();
		List<Tile> actualRevealedDoraIndicators= [];
		if (numberOfDoraTilesToReveal == -1)
		{
			while(createdWall.CanRevealDoraIndicator)
			{
				var revealedDoraIndicator = createdWall.RevealNextDoraIndicator();
				actualRevealedDoraIndicators.Add(revealedDoraIndicator);
				PrefixInfo("Checking that just revealed dora indicator is the same as accessing it through its index...");
				AssertObject(wallTiles[createdWall.LastRevealedDoraIndicatorIndex]).IsSame(revealedDoraIndicator);
			}
		}
		else
		{
			for (int i = 0; i < numberOfDoraTilesToReveal; i++)
			{
				var revealedDoraIndicator = createdWall.RevealNextDoraIndicator();
				actualRevealedDoraIndicators.Add(revealedDoraIndicator);
				PrefixInfo("Checking that just revealed dora indicator is the same as accessing it through its index...");
				AssertObject(wallTiles[createdWall.LastRevealedDoraIndicatorIndex]).IsSame(revealedDoraIndicator);
			}
		}

		PrefixInfo($"DEBUG: Actual Revealed Dora Indicators Tiles: \"{actualRevealedDoraIndicators.NotationFromTiles()}\"");
		PrefixInfo("Checking that actual revealed dora indicators matched expected revealed dora indicators...");
		AssertArray(expectedRevealedDoraIndicators).ContainsExactly(actualRevealedDoraIndicators);

		PrefixInfo("Checking that all revealed dora indicators are face-up");
		AssertThat(actualRevealedDoraIndicators.All(t => t.FaceUp = true));

		var doraIndicatorsExhausted = numberOfDoraTilesToReveal == -1 || numberOfDoraTilesToReveal == 4;
		if (doraIndicatorsExhausted)
		{
			PrefixInfo("Checking that CanRevealDoraIndicator reports that a dora can't be revealed after exhaustion...");
			AssertThat(createdWall.CanRevealDoraIndicator).IsFalse();

			var exhaustedDoraIndicator = createdWall.RevealNextDoraIndicator();
			PrefixInfo("Checking that RevealNextDoraIndicator returns null...");
			AssertObject(exhaustedDoraIndicator).IsNull();
		}
		else
		{
			PrefixInfo("Checking that CanRevealDoraIndicator reports that a dora can still be revealed after revealing less than the available number of indicators...");
			AssertThat(createdWall.CanTakeReplacementTile).IsTrue();
		}

		var lastRevealedDoraIndicator = wallTiles[createdWall.LastRevealedDoraIndicatorIndex];
		PrefixInfo("Checking that last revealed dora indicator is the same as accessing it through its index...");
		AssertObject(wallTiles[createdWall.LastRevealedDoraIndicatorIndex]).IsSame(lastRevealedDoraIndicator);
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
		var currentIndex = expectedStartingEastIndex;
		foreach (var currentWind in Enum.GetValues<Wind>())
		{
			if ((playerCount == PlayerCount.Three && currentWind == Wind.North) ||
				(playerCount == PlayerCount.Two && currentWind == Wind.West))
			{
				break;
			}

			var currentPlayer = actualPlayers[currentIndex];
			PrefixInfo($"Checking that the player at index {currentIndex} is assigned wind {currentWind}...");
			AssertObject(createdMatch.WindsToPlayers[currentWind]).IsSame(currentPlayer);

			currentIndex = GetSafeIndex(++currentIndex, actualPlayers.Count);
		}
	}

	private static IEnumerable<object[]> WallCreationTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			false, // Red Fives?
			2, // Dice Roll
			38, // Live Wall Start, Next Live Tile
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
			94, // Next Replacement Tile
			86, // Last Revealed Dora Indicator
			84, // Next Dora Indicator
			"3z".ToTile(), // Expected Revealed Dora
		];
	}

	private static IEnumerable<object[]> PopLiveTileTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			true,
			("1z7614p7m172z14m7z4m1p2m7s5p0m9p6s2z9m87p1z7s6m73p8m5s6p2z6p26s4z1s4z0p3s89p469s5m7p064s3z2m1p8m4s66z" +
			"2p5s1m1p8s82p8s4m12z7m54z617m2p7s6z5p4m5p9m9p4s3z3p3m5z7s6z2s3m3z31s836m3p21m4p1s28m3s74z5m8s6m1s73z92p" +
			"7m3p9s9m2s484p389s9m95s6p55z2s35m").ToTiles(),
		];
		yield return
		[
			PlayerCount.Two,
			false,
			("1z7614p7m172z14m7z4m1p2m7s5p5m9p6s2z9m87p1z7s6m73p8m5s6p2z6p26s4z1s4z5p3s89p469s5m7p564s3z2m1p8m4s66z" +
			"2p5s1m1p8s82p8s4m12z7m54z617m2p7s6z5p4m5p9m9p4s3z3p3m5z7s6z2s3m3z31s836m3p21m4p1s28m3s74z5m8s6m1s73z92p" +
			"7m3p9s9m2s484p389s9m95s6p55z2s35m").ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			true,
			("82s3z179s5z5s6z7p6s9p7s7p8s1z1m5p7s5z4p6z1m2p37s4z7p9m5z9s7z5s9m4z64s0138p91m8p4s9m4p6s74z2p1z3p0s73z3s" +
			"748p2z6p5s9p2s8p6s11z5p2z81s65z4s12p2z21s694p373z1m1p6z261p91s3p48s4z6p32s2z9s3p3s95p").ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			false,
			("82s3z179s5z5s6z7p6s9p7s7p8s1z1m5p7s5z4p6z1m2p37s4z7p9m5z9s7z5s9m4z64s5138p91m8p4s9m4p6s74z2p1z3p5s73z3s" +
			"748p2z6p5s9p2s8p6s11z5p2z81s65z4s12p2z21s694p373z1m1p6z261p91s3p48s4z6p32s2z9s3p3s95p").ToTiles(),
		];
	}

	private static IEnumerable<object[]> PopReplacementTileTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			true,
			-1, // Take to exhaustion (4)
			"3m2s55z".ToTiles(),
			("1z7614p7m172z14m7z4m1p2m7s5p0m9p6s2z9m87p1z7s6m73p8m5s6p2z6p26s4z1s4z0p3s89p469s5m7p064s3z2m1p8m4s66z" +
			"2p5s1m1p8s82p8s4m12z7m54z617m2p7s6z5p4m5p9m9p4s3z3p3m5z7s6z2s3m3z31s836m3p21m4p1s28m3s74z5m8s6m1s73z92p" +
			"7m3p9s9m2s484p389s9m95s6p5z").ToTiles(),
		];
		yield return
		[
			PlayerCount.Two,
			false,
			2,
			"3m2s".ToTiles(),
			("1z7614p7m172z14m7z4m1p2m7s5p5m9p6s2z9m87p1z7s6m73p8m5s6p2z6p26s4z1s4z5p3s89p469s5m7p564s3z2m1p8m4s66z" +
			"2p5s1m1p8s82p8s4m12z7m54z617m2p7s6z5p4m5p9m9p4s3z3p3m5z7s6z2s3m3z31s836m3p21m4p1s28m3s74z5m8s6m1s73z92p" +
			"7m3p9s9m2s484p389s9m95s6p55z2s").ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			true,
			-1, // Take to exhaustion (8),
			"9p3s3p9s2z23s6p".ToTiles(),
			("82s3z179s5z5s6z7p6s9p7s7p8s1z1m5p7s5z4p6z1m2p37s4z7p9m5z9s7z5s9m4z64s0138p91m8p4s9m4p6s74z2p1z3p0s73z3s" +
			"748p2z6p5s9p2s8p6s11z5p2z81s65z4s12p2z21s694p373z1m1p6z261p91s3p48s4z6p").ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			false,
			5,
			"9p3s3p9s2z".ToTiles(),
			("82s3z179s5z5s6z7p6s9p7s7p8s1z1m5p7s5z4p6z1m2p37s4z7p9m5z9s7z5s9m4z64s5138p91m8p4s9m4p6s74z2p1z3p5s73z3s" +
			"748p2z6p5s9p2s8p6s11z5p2z81s65z4s12p2z21s694p373z1m1p6z261p91s3p48s4z6p32s2z").ToTiles(),
		];
	}

	private static IEnumerable<object[]> RevealDoraIndicatorTestCases()
	{
		yield return
		[
			PlayerCount.Four,
			true,
			-1, // Take to exhaustion (4)
			"993s8p".ToTiles(),
		];
		yield return
		[
			PlayerCount.Two,
			false,
			2,
			"99s".ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			true,
			-1, // Take to exhaustion (4),
			"41s12p".ToTiles(),
		];
		yield return
		[
			PlayerCount.Three,
			false,
			3,
			"41s1p".ToTiles(),
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