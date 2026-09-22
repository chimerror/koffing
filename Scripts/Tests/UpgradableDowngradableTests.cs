using System;
using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class UpgradableDowngradableTests
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
	[DataPoint(nameof(DowngradeChowToKanchanTestCases))]
	public static void DowngradeChowToKanchanIsCorrect(Chow chow, IEnumerable<MadeBlockContext> expectedOutput)
	{
		LoggingPrefix = nameof(DowngradeChowToKanchanIsCorrect);

		var actualOutput = ((IDowngradable<Kanchan>)chow).Downgrade();
		PrefixInfo($"Checking that downgrading chow \"{chow.Tiles.NotationFromTiles()}\" to kanchan works");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(DowngradeChowToPenchanTestCases))]
	public static void DowngradeChowToPenchanIsCorrect(Chow chow, IEnumerable<MadeBlockContext> expectedOutput, string because)
	{
		LoggingPrefix = nameof(DowngradeChowToPenchanIsCorrect);

		var actualOutput = ((IDowngradable<Penchan>)chow).Downgrade();
		PrefixInfo($"Checking that downgrading chow \"{chow.Tiles.NotationFromTiles()}\" to penchan works when given {because}");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(DowngradeChowToRyanmenTestCases))]
	public static void DowngradeChowToRyanmenIsCorrect(Chow chow, IEnumerable<MadeBlockContext> expectedOutput)
	{
		LoggingPrefix = nameof(DowngradeChowToRyanmenIsCorrect);

		var actualOutput = ((IDowngradable<Ryanmen>)chow).Downgrade();
		PrefixInfo($"Checking that downgrading chow \"{chow.Tiles.NotationFromTiles()}\" to ryanmen works");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	private static IEnumerable<object[]> DowngradeChowToKanchanTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			if (suit == Suit.Zi)
			{
				continue;
			}

			for (int lowRank = 1; lowRank <= 7; lowRank++)
			{
				var lowTile = new Tile(suit, lowRank);
				var middleTile = new Tile(suit, lowRank + 1);
				var highTile = new Tile(suit, lowRank + 2);
				yield return
				[
					new Chow([lowTile, middleTile, highTile]),
					new List<MadeBlockContext>()
					{
						new(new Kanchan([lowTile, highTile]), [middleTile]),
					},
				];
			}
		}
	}

	private static IEnumerable<object[]> DowngradeChowToPenchanTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			if (suit == Suit.Zi)
			{
				continue;
			}

			var suitString = suit.GetSuitString();

			yield return
			[
				new Chow($"123{suitString}".ToTiles()),
				new List<MadeBlockContext>()
				{
					new(new Penchan($"12{suitString}".ToTiles()), $"3{suitString}".ToTiles()),
				},
				$"a starting tile of 1",
			];
			yield return
			[
				new Chow($"789{suitString}".ToTiles()),
				new List<MadeBlockContext>()
				{
					new(new Penchan($"89{suitString}".ToTiles()), $"7{suitString}".ToTiles()),
				},
				$"a starting tile of 7",
			];
			yield return
			[
				new Chow($"456{suitString}".ToTiles()),
				new List<MadeBlockContext>()
				{
				},
				$"a non-terminal",
			];
		}
	}

	private static IEnumerable<object[]> DowngradeChowToRyanmenTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			if (suit == Suit.Zi)
			{
				continue;
			}

			for (int lowRank = 1; lowRank <= 7; lowRank++)
			{
				// This is pretty straightly re-implementing actual implementation, but not really an easy way to test
				// this otherwise.
				var lowTile = new Tile(suit, lowRank);
				var middleTile = new Tile(suit, lowRank + 1);
				var highTile = new Tile(suit, lowRank + 2);
				List<MadeBlockContext> expectedMadeBlocks = [];
				if (lowTile.Rank > 1)
				{
					expectedMadeBlocks.Add(new MadeBlockContext(new Ryanmen([lowTile, middleTile]), [highTile]));
				}
				if (middleTile.Rank < 8)
				{
					expectedMadeBlocks.Add(new MadeBlockContext(new Ryanmen([middleTile, highTile]), [lowTile]));
				}
				yield return
				[
					new Chow([lowTile, middleTile, highTile]),
					expectedMadeBlocks,
				];
			}
		}
	}
}
