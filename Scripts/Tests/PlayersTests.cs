using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using Koffing.Players;
using Koffing.Tiles;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

namespace Koffing.Tests;

[TestSuite]
public class PlayersTests
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
	public static void PlayerStatePerformDrawWorksCorrectly()
	{
		LoggingPrefix = nameof(PlayerStatePerformDrawWorksCorrectly);

		PrefixInfo("Checking that PlayerState can be created...");
		PlayerState playerState = new();
		AssertObject(playerState).IsNotNull();

		var handTiles = "123m456p789s1457z".ToTiles().ToList();
		playerState.Hand = handTiles;

		var tileToDraw = "3z".ToTile();
		playerState.PerformDraw(tileToDraw);

		PrefixInfo("Checking that tile was drawn...");
		AssertThat(playerState.DrawnTile).IsEqual(tileToDraw);

		PrefixInfo("Checking that tile was not added to hand...");
		AssertArray(handTiles).ContainsExactly(playerState.Hand);
		AssertThat(playerState.Hand.Any(t => t == tileToDraw)).IsFalse();
	}

	[TestCase]
	[ThrowsException(typeof(ArgumentException))]
	public static void PlayerStateCannotPerformDrawWithNullTile()
	{
		LoggingPrefix = nameof(PlayerStateCannotPerformDrawWithNullTile);

		PlayerState playerState = new();
		PrefixInfo("Checking that PlayerState throws when trying to draw null tile...");
		playerState.PerformDraw(null); // Should Throw
	}

	[TestCase]
	[ThrowsException(typeof(InvalidOperationException))]
	public static void PlayerStateCannotDrawTileWhenTileAlreadyDrawn()
	{
		LoggingPrefix = nameof(PlayerStateCannotDrawTileWhenTileAlreadyDrawn);

		PlayerState playerState = new();
		var tileToDraw = "3z".ToTile();
		playerState.PerformDraw(tileToDraw);

		PrefixInfo("Checking that PlayerState throws when trying to draw tile when tile has already been drawn...");
		var anotherTileToDraw = "4m".ToTile();
		playerState.PerformDraw(anotherTileToDraw); // Should Throw
	}

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformDiscardPositiveTestCases))]
	public static void PlayerStatePerformDiscardWorksCorrectly(
		string handNotation,
		Tile drawnTile,
		string discardsNotation,
		int discardIndex,
		bool isRiichiDiscard,
		Tile expectedDiscard,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformDiscardWorksCorrectly);

		var startingHand = handNotation.ToTiles().ToList();
		var startingDiscards = discardsNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
			DrawnTile = drawnTile,
			Discards = [.. startingDiscards],
		};

		var riichiOutcomeString = isRiichiDiscard ? "as an riichi discard" : "as a regular discard";
		PrefixInfo($"Checking that discarding index {discardIndex} {riichiOutcomeString} causes \"{expectedDiscard.NotationFromTile()}\" to be discarded with hand \"{handNotation}\", drawn tile \"{drawnTile.NotationFromTile()}\", and discards \"{discardsNotation}\" when {because}...");
		playerState.PerformDiscard(discardIndex, isRiichiDiscard);
		AssertObject(playerState.DrawnTile).IsNull();

		List<Tile> expectedHand = [.. startingHand];
		if (discardIndex != -1)
		{
			expectedHand.RemoveAt(discardIndex);
			expectedHand.Add(drawnTile);
		}
		AssertArray(expectedHand).ContainsExactly(playerState.Hand);

		List<Tile> expectedDiscards = [.. startingDiscards, expectedDiscard];
		AssertArray(expectedDiscards).ContainsExactly(playerState.Discards);
		AssertThat(playerState.Discards.Last().Rotated).IsEqual(isRiichiDiscard);
	}

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformDiscardNegativeTestCases))]
	[ThrowsException(typeof(ArgumentException))]
	public static void PlayerStatePerformDiscardRejectsBadArguments(
		string handNotation,
		int discardIndex,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformDiscardRejectsBadArguments);

		var startingHand = handNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
		};

		PrefixInfo($"Checking that ArgumentException is thrown with hand \"{handNotation}\" when given {because}...");
		playerState.PerformDiscard(discardIndex); // Should throw
	}

	[TestCase]
	[ThrowsException(typeof(InvalidOperationException))]
	public static void PlayerStatePerformDiscardRejectsDiscardingNonExistentDrawnTile()
	{
		LoggingPrefix = nameof(PlayerStatePerformDiscardRejectsDiscardingNonExistentDrawnTile);

		PlayerState playerState = new()
		{
			Hand = [.. "123456789m1234z".ToTiles()],
		};

		PrefixInfo("Checking that InvalidOperationException is thrown when trying to discard non-existent drawn tile...");
		playerState.PerformDiscard(-1);
	}

	private static IEnumerable<object[]> PlayerStatePerformDiscardPositiveTestCases()
	{
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			-1,
			false,
			"5p".ToTile(),
			"discarding the drawn tile as a regular discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			-1,
			true,
			"5p".ToTile(),
			"discarding the drawn tile as a riichi discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			0,
			false,
			"1m".ToTile(),
			"discarding the first hand tile as a regular discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			0,
			true,
			"1m".ToTile(),
			"discarding the first hand tile as a riichi discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			6,
			false,
			"7m".ToTile(),
			"discarding a middle hand tile as a regular discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			6,
			true,
			"7m".ToTile(),
			"discarding a middle hand tile as a riichi discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			12,
			false,
			"4z".ToTile(),
			"discarding the last hand tile as a regular discard",
		];
		yield return
		[
			"123456789m1234z",
			"5p".ToTile(),
			"987654321s",
			12,
			true,
			"4z".ToTile(),
			"discarding the last hand tile as a riichi discard",
		];
	}

	private static IEnumerable<object[]> PlayerStatePerformDiscardNegativeTestCases()
	{
		yield return
		[
			"123456789m1234z",
			-3,
			"a discard index below -1",
		];
		yield return
		[
			"123456789m1234z",
			13,
			"a discard index the same as the hand count",
		];
		yield return
		[
			"123456789m1234z",
			17,
			"a discard index past the hand count",
		];
	}
}