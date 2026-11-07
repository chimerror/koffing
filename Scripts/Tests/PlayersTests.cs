using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using Koffing.Melds;
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

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformMeldPositiveTestCases))]
	public static void PlayerStatePerformMeldWorksCorrectly(
		string handNotation,
		Tile drawnTile,
		List<Meld> startingMelds,
		string discardsNotation,
		Meld newlyMadeMeld,
		string expectedPostMeldHandNotation,
		List<Meld> expectedPostMeldMelds,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformMeldWorksCorrectly);

		var startingHand = handNotation.ToTiles().ToList();
		var startingDiscards = discardsNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
			DrawnTile = drawnTile,
			MadeMelds = [.. startingMelds],
			Discards = [.. startingDiscards],
		};

		PrefixInfo($"Checking possible to meld meld {newlyMadeMeld} with hand \"{handNotation}\" and drawn tile \"{drawnTile}\", given {because}");
		playerState.PerformMeld(newlyMadeMeld);
		AssertObject(playerState.DrawnTile).IsNull();
		AssertArray(playerState.MadeMelds).ContainsExactly(expectedPostMeldMelds);
		AssertArray(playerState.Discards).ContainsExactly(startingDiscards);

		var expectedPostMeldHand = expectedPostMeldHandNotation.ToTiles().ToList();
		AssertThat(playerState.Hand).IsEqual(expectedPostMeldHand);
	}

	[TestCase]
	[ThrowsException(typeof(ArgumentException))]
	public static void PlayerStatePerformMeldRejectsNullMeld()
	{
		LoggingPrefix = nameof(PlayerStatePerformMeldRejectsNullMeld);

		PlayerState playerState = new();
		playerState.PerformMeld(null);
	}

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformMeldNegativeTestCases))]
	[ThrowsException(typeof(InvalidOperationException))]
	public static void PlayerStatePerformMeldRejectsInvalidMelds(
		string handNotation,
		Tile drawnTile,
		List<Meld> startingMelds,
		Meld newlyMadeMeld,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformMeldRejectsInvalidMelds);

		var startingHand = handNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
			MadeMelds = [.. startingMelds],
			DrawnTile = drawnTile,
		};

		PrefixInfo($"Checking that attempting to perform meld {newlyMadeMeld} with hand \"{handNotation}\" and drawn tile \"{drawnTile}\" results in throwing InvalidOperationException, because {because}");
		playerState.PerformMeld(newlyMadeMeld); // Should throw
	}

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformPeiDoraClaimPositiveTestCases))]
	public static void PlayerStatePerformPeiDoraClaimWorksCorrectly(
		string handNotation,
		Tile drawnTile,
		int startingNumberOfPeiDora,
		string expectedPostMeldHandNotation,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformPeiDoraClaimWorksCorrectly);

		var startingHand = handNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
			DrawnTile = drawnTile,
			PeiDora = [.. Enumerable.Range(1, startingNumberOfPeiDora).Select(_ => new Tile(Suit.Zi, 4))]
		};

		PrefixInfo($"Checking that it is possible to claim a pei dora with hand \"{handNotation}\", drawn tile \"{drawnTile}\", and {startingNumberOfPeiDora} starting pei dora, given {because}...");
		playerState.PerformPeiDoraClaim();
		AssertThat(playerState.PeiDora.Count).IsEqual(startingNumberOfPeiDora + 1);
		AssertThat(playerState.PeiDora.All(t => t.Suit == Suit.Zi && t.Rank == 4)).IsTrue();
		AssertObject(playerState.DrawnTile).IsNull();
		AssertArray(playerState.MadeMelds).IsEmpty();
		AssertArray(playerState.Discards).IsEmpty();

		var expectedPostMeldHand = expectedPostMeldHandNotation.ToTiles().ToList();
		AssertThat(playerState.Hand).IsEqual(expectedPostMeldHand);
	}

	[TestCase]
	[DataPoint(nameof(PlayerStatePerformPeiDoraClaimNegativeTestCases))]
	[ThrowsException(typeof(InvalidOperationException))]
	public static void PlayerStatePerformPeiDoraClaimRejectsInvalidMelds(
		string handNotation,
		Tile drawnTile,
		int startingNumberOfPeiDora,
		string because)
	{
		LoggingPrefix = nameof(PlayerStatePerformPeiDoraClaimRejectsInvalidMelds);

		var startingHand = handNotation.ToTiles().ToList();
		PlayerState playerState = new()
		{
			// Use collection syntax to force new separate lists
			Hand = [.. startingHand],
			DrawnTile = drawnTile,
			PeiDora = [.. Enumerable.Range(1, startingNumberOfPeiDora).Select(_ => new Tile(Suit.Zi, 4))]
		};

		PrefixInfo($"Checking that attempting to claim a pei dora with hand \"{handNotation}\", drawn tile \"{drawnTile}\", and {startingNumberOfPeiDora} starting pei dora, throws an InvalidOperation because {because}...");
		playerState.PerformPeiDoraClaim(); // Should throw
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

	private static IEnumerable<object[]> PlayerStatePerformMeldPositiveTestCases()
	{
		yield return
		[
			"19m19p19s1234444z",
			"3z".ToTile(),
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			"19m19p19s1233z",
			new List<Meld>()
			{
				new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			},
			"concealed kong fully in hand",
		];

		yield return
		[
			"19m19p19s12334444z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			"19m19p19s1233z",
			new List<Meld>()
			{
				new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			},
			"concealed kong fully in hand with null drawn tile",
		];

		yield return
		[
			"19m19p19s1233444z",
			"4z".ToTile(),
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			"19m19p19s1233z",
			new List<Meld>()
			{
				new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			},
			"concealed kong from drawn tile",
		];

		yield return
		[
			"19m19p15559s1233z",
			"0s".ToTile(),
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("5X505Xs".ToTiles(), Kong.Type.Concealed),
			"19m19p19s1233z",
			new List<Meld>()
			{
				new Kong("5X505Xs".ToTiles(), Kong.Type.Concealed),
			},
			"concealed kong using drawn red five",
		];

		yield return
		[
			"19m19p150559s123z",
			"1m".ToTile(),
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("5X505Xs".ToTiles(), Kong.Type.Concealed),
			"19m19p19s123z1m",
			new List<Meld>()
			{
				new Kong("5X055Xs".ToTiles(), Kong.Type.Concealed),
			},
			"concealed kong with red five fully in hand",
		];

		yield return
		[
			"19m19p19s1236z",
			"4z".ToTile(),
			new List<Meld>()
			{
				new Pung("4'44z".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"19m19p19s1236z",
			new List<Meld>()
			{
				new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with drawn tile from only pung",
		];

		yield return
		[
			"19m19p19s1234z",
			"3m".ToTile(),
			new List<Meld>()
			{
				new Pung("4'44z".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"19m19p19s123z3m",
			new List<Meld>()
			{
				new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with in-hand tile from only pung",
		];

		yield return
		[
			"179m19p19s1234z",
			null,
			new List<Meld>()
			{
				new Pung("4'44z".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"179m19p19s123z",
			new List<Meld>()
			{
				new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with in-hand tile and null drawn tile from only pung",
		];

		yield return
		[
			"19m19p19s1234z",
			"0s".ToTile(),
			new List<Meld>()
			{
				new Pung("5'55s".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("5'0\"55s".ToTiles(), Kong.Type.Promoted),
			"19m19p19s1234z",
			new List<Meld>()
			{
				new Kong("5'0\"55s".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with drawn red five from only pung",
		];

		yield return
		[
			"19m19p109s123z",
			"3m".ToTile(),
			new List<Meld>()
			{
				new Pung("5'55s".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("5'0\"55s".ToTiles(), Kong.Type.Promoted),
			"19m19p19s123z3m",
			new List<Meld>()
			{
				new Kong("5'0\"55s".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with in-hand red five from only pung",
		];

		yield return
		[
			"19m19p19s1234z",
			"5s".ToTile(),
			new List<Meld>()
			{
				new Pung("0'55s".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("0'5\"55s".ToTiles(), Kong.Type.Promoted),
			"19m19p19s1234z",
			new List<Meld>()
			{
				new Kong("0'5\"55s".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with drawn non-red five from only pung starting with red five",
		];

		yield return
		[
			"19m19p12345z5s",
			"3m".ToTile(),
			new List<Meld>()
			{
				new Pung("0'55s".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("5'0\"55s".ToTiles(), Kong.Type.Promoted),
			"19m19p12345z3m",
			new List<Meld>()
			{
				new Kong("0'5\"55s".ToTiles(), Kong.Type.Promoted),
			},
			"promoted kong with in-hand non-red five from only pung starting with red five",
		];

		yield return
		[
			"1234z",
			"3m".ToTile(),
			new List<Meld>()
			{
				new Pung("4'44z".ToTiles()),
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"123z3m",
			new List<Meld>()
			{
				new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"promoted kong with in-hand tile from first pung",
		];

		yield return
		[
			"3m123z",
			"4z".ToTile(),
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Pung("4'44z".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"3m123z",
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
				new Chow("460'm".ToTiles()),
			},
			"promoted kong with drawn tile from middle pung",
		];

		yield return
		[
			"19m19p19s1234445z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("444'4z".ToTiles(), Kong.Type.Called),
			"19m19p19s1235z",
			new List<Meld>()
			{
				new Kong("444'4z".ToTiles(), Kong.Type.Called),
			},
			"called kong fully in hand",
		];

		yield return
		[
			"19m15509p19s1237z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("5055'p".ToTiles(), Kong.Type.Called),
			"19m19p19s1237z",
			new List<Meld>()
			{
				new Kong("5055'p".ToTiles(), Kong.Type.Called),
			},
			"called kong with in-hand red five",
		];

		yield return
		[
			"19m15559p19s1233z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Kong("0'555p".ToTiles(), Kong.Type.Called),
			"19m19p19s1233z",
			new List<Meld>()
			{
				new Kong("0'555p".ToTiles(), Kong.Type.Called),
			},
			"called kong of off red five (as starting tile of meld)",
		];

		yield return
		[
			"3m123444z",
			null,
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Kong("444'4z".ToTiles(), Kong.Type.Called),
			"3m123z",
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
				new Kong("444'4z".ToTiles(), Kong.Type.Called),
			},
			"called kong with other melds",
		];

		yield return
		[
			"19m1479p19s12344z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Pung("44'4z".ToTiles()),
			"19m1479p19s123z",
			new List<Meld>()
			{
				new Pung("44'4z".ToTiles()),
			},
			"called pung",
		];

		yield return
		[
			"1059m19p19s12347z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Pung("5'05m".ToTiles()),
			"19m19p19s12347z",
			new List<Meld>()
			{
				new Pung("5'05m".ToTiles()),
			},
			"called pung with a red five in hand",
		];

		yield return
		[
			"1559m19p19s12335z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Pung("50'5m".ToTiles()),
			"19m19p19s12335z",
			new List<Meld>()
			{
				new Pung("50'5m".ToTiles()),
			},
			"called pung off of red five",
		];

		yield return
		[
			"3m2p12344z",
			null,
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Pung("444'z".ToTiles()),
			"3m2p123z",
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
				new Pung("444'z".ToTiles()),
			},
			"called pung with other melds",
		];

		yield return
		[
			"19m1129p19s12345z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Chow("123'p".ToTiles()),
			"19m19p19s12345z",
			new List<Meld>()
			{
				new Chow("123'p".ToTiles()),
			},
			"called chow",
		];

		yield return
		[
			"19m1409p19s12345z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Chow("3'40p".ToTiles()),
			"19m19p19s12345z",
			new List<Meld>()
			{
				new Chow("3'40p".ToTiles()),
			},
			"called chow with red five in hand",
		];

		yield return
		[
			"19m1349p19s12345z",
			null,
			new List<Meld>(),
			"765z23s4m6p7z8p",
			new Chow("30'4p".ToTiles()),
			"19m19p19s12345z",
			new List<Meld>()
			{
				new Chow("30'4p".ToTiles()),
			},
			"called chow off of red five",
		];

		yield return
		[
			"79m2p1234z",
			null,
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			"765z23s4m6p7z8p",
			new Chow("8'79m".ToTiles()),
			"2p1234z",
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Chow("460'm".ToTiles()),
				new Chow("8'79m".ToTiles()),
			},
			"called pung with other melds",
		];
	}

	private static IEnumerable<object[]> PlayerStatePerformMeldNegativeTestCases()
	{
		yield return
		[
			"19m19p19s123444z",
			"3z".ToTile(),
			new List<Meld>(),
			new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			"there're not enough tiles in hand for a concealed kong",
		];

		yield return
		[
			"19m19p19s122344z",
			"4z".ToTile(),
			new List<Meld>(),
			new Kong("4X444Xz".ToTiles(), Kong.Type.Concealed),
			"there're not enough tiles between hand and drawn tiles for a concealed kong",
		];

		yield return
		[
			"3m123z",
			"6z".ToTile(),
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Pung("4'44z".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"there're not enough tiles between hand and drawn tiles for a promoted kong",
		];

		yield return
		[
			"3m123z",
			"4z".ToTile(),
			new List<Meld>()
			{
				new Pung("88'8s".ToTiles()),
				new Pung("5'55z".ToTiles()),
				new Chow("460'm".ToTiles()),
			},
			new Kong("4'4\"44z".ToTiles(), Kong.Type.Promoted),
			"there's no matching pung for a promoted kong",
		];

		yield return
		[
			"19m19p19s1223445z",
			null,
			new List<Meld>(),
			new Kong("444'4z".ToTiles(), Kong.Type.Called),
			"there're not enough tiles for a called kong",
		];

		yield return
		[
			"19m19p19s1222345z",
			null,
			new List<Meld>(),
			new Pung("44'4z".ToTiles()),
			"there're not enough tiles for a called pung",
		];

		yield return
		[
			"19m119p19s12345z",
			null,
			new List<Meld>(),
			new Chow("123'p".ToTiles()),
			"there are missing tiles for a called chow",
		];
	}

	private static IEnumerable<object[]> PlayerStatePerformPeiDoraClaimPositiveTestCases()
	{
		yield return
		[
			"19m19p19s1234567z",
			"3m".ToTile(),
			0,
			"19m19p19s123567z3m",
			"a single in-hand north tile",
		];

		yield return
		[
			"139m19p19s123456z",
			null,
			0,
			"139m19p19s12356z",
			"a single in-hand north tile with null drawn tile",
		];

		yield return
		[
			"19m19p19s1233567z",
			"4z".ToTile(),
			0,
			"19m19p19s1233567z",
			"a single just drawn north tile",
		];

		yield return
		[
			"19m19p19s1234467z",
			"3m".ToTile(),
			0,
			"19m19p19s123467z3m",
			"multiple north tiles in hand with none claimed",
		];

		yield return
		[
			"19m19p19s1234467z",
			"4z".ToTile(),
			0,
			"19m19p19s1234674z",
			"multiple north tiles in hand and drawn with none claimed",
		];

		yield return
		[
			"19m19p19s1234567z",
			"3m".ToTile(),
			1,
			"19m19p19s123567z3m",
			"a single in-hand north tile with one claimed",
		];

		yield return
		[
			"19m19p19s1234567z",
			"3m".ToTile(),
			2,
			"19m19p19s123567z3m",
			"a single in-hand north tile with two claimed",
		];

		yield return
		[
			"19m19p19s1234567z",
			"3m".ToTile(),
			3,
			"19m19p19s123567z3m",
			"a single in-hand north tile with three claimed",
		];
	}

	private static IEnumerable<object[]> PlayerStatePerformPeiDoraClaimNegativeTestCases()
	{
		yield return
		[
			"19m19p19s1233567z",
			"3m".ToTile(),
			0,
			"there aren't any north tiles in hand or the drawn tile",
		];

		yield return
		[
			"19m19p19s1234567z",
			"3m".ToTile(),
			4,
			"there are already 4 claimed pei dora",
		];
	}
}