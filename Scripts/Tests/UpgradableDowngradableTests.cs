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

	[TestCase]
	[DataPoint(nameof(DowngradeKongToPungTestCases))]
	public static void DowngradeKongToPungIsCorrect(Kong kong, IEnumerable<MadeBlockContext> expectedOutput)
	{
		LoggingPrefix = nameof(DowngradeKongToPungIsCorrect);

		var actualOutput = ((IDowngradable<Pung>)kong).Downgrade();
		PrefixInfo($"Checking that downgrading kong \"{kong.Tiles.NotationFromTiles()}\" to pung works");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(DowngradePungToPairTestCases))]
	public static void DowngradePungToPairIsCorrect(Pung pung, IEnumerable<MadeBlockContext> expectedOutput)
	{
		LoggingPrefix = nameof(DowngradePungToPairIsCorrect);

		var actualOutput = ((IDowngradable<Pair>)pung).Downgrade();
		PrefixInfo($"Checking that downgrading pung \"{pung.Tiles.NotationFromTiles()}\" to pair works");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(UpgradePungToKongSoughtTilesTestCases))]
	public static void UpgradePungToKongSoughtTilesIsCorrect(Pung pung, HashSet<Tile> expectedOutput)
	{
		LoggingPrefix = nameof(UpgradePungToKongSoughtTilesIsCorrect);

		var actualOutput = ((IUpgradable<Kong>)pung).SoughtTiles;
		PrefixInfo($"Checking that tiles sought for upgrading pung \"{pung.Tiles.NotationFromTiles()}\" to kong works");
		AssertArray(expectedOutput).ContainsExactlyInAnyOrder(actualOutput);
	}

	[TestCase]
	[DataPoint(nameof(UpgradePungToKongCanUpgradeWithTestCases))]
	public static void UpgradePungToKongCanUpgradeWithIsCorrect(Pung pung, Tile tile, bool expectedResult, string because)
	{
		LoggingPrefix = nameof(UpgradePungToKongCanUpgradeWithIsCorrect);

		var actualResult = ((IUpgradable<Kong>)pung).CanUpgradeWith(tile);
		var outcomeString = expectedResult ? "can" : "can NOT";
		PrefixInfo($"Checking that tile \"{tile}\" {outcomeString} upgrade pung \"{pung.Tiles.NotationFromTiles()}\" to kong when given {because}");
		AssertThat(expectedResult).IsEqual(actualResult);
	}

	[TestCase]
	[ThrowsException(typeof(ArgumentException))]
	[DataPoint(nameof(UpgradePungToKongNegativeTestCases))]
	public static void UpgradePungToKongNegativeCasesAreCorrect(Pung pung, Tile tile, string because)
	{
		LoggingPrefix = nameof(UpgradePungToKongNegativeCasesAreCorrect);

		PrefixInfo($"Checking that trying upgrade pung \"{pung.Tiles.NotationFromTiles()}\" to kong with tile \"{tile}\" throws an ArgumentException when given {because}");
		((IUpgradable<Kong>)pung).Upgrade(tile); // will throw
	}

	[TestCase]
	[DataPoint(nameof(UpgradePungToKongPositiveTestCases))]
	public static void UpgradePungToKongPositiveCasesAreCorrect(Pung pung, Tile tile, Kong expectedOutput, string because)
	{
		LoggingPrefix = nameof(UpgradePungToKongPositiveCasesAreCorrect);

		var actualOutput = ((IUpgradable<Kong>)pung).Upgrade(tile); // will throw
		PrefixInfo($"Checking that it is possible to upgrade pung \"{pung.Tiles.NotationFromTiles()}\" to kong with tile \"{tile}\" when given {because}");
		AssertThat(expectedOutput).IsEqual(actualOutput);
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

	private static IEnumerable<object[]> DowngradeKongToPungTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				if (suit != Suit.Zi && rank == 5)
				{
					var nonRedFives = $"555{suitString}".ToTiles();
					var redFive = $"0{suitString}".ToTile();
					yield return
					[
						new Kong(nonRedFives.Append(redFive)),
						new List<MadeBlockContext>()
						{
							new(new Pung(nonRedFives), [redFive]),
							new(new Pung(nonRedFives.Take(2).Append(redFive)), nonRedFives.Skip(2)),
						},
					];
				}
				else
				{
					var tiles = $"{rank}{rank}{rank}{rank}{suitString}".ToTiles();
					yield return
					[
						new Kong(tiles),
						new List<MadeBlockContext>()
						{
							new(new Pung(tiles.Take(3)), tiles.Skip(3)),
						},
					];
				}
			}
		}
	}

	private static IEnumerable<object[]> DowngradePungToPairTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				if (suit != Suit.Zi && rank == 5)
				{
					var nonRedFives = $"555{suitString}".ToTiles();
					var redFive = $"0{suitString}".ToTile();
					yield return
					[
						new Pung(nonRedFives),
						new List<MadeBlockContext>()
						{
							new(new Pair(nonRedFives.Take(2)), nonRedFives.Skip(2)),
						}
					];
					yield return
					[
						new Pung(nonRedFives.Take(2).Append(redFive)),
						new List<MadeBlockContext>()
						{
							new(new Pair(nonRedFives.Take(2)), [redFive]),
							new(new Pair(nonRedFives.Take(1).Append(redFive)), nonRedFives.Skip(1).Take(1)),
						}
					];
				}
				else
				{
					var tiles = $"{rank}{rank}{rank}{suitString}".ToTiles();
					yield return
					[
						new Pung(tiles),
						new List<MadeBlockContext>()
						{
							new(new Pair(tiles.Take(2)), tiles.Skip(2)),
						},
					];
				}
			}
		}
	}

	private static IEnumerable<object[]> UpgradePungToKongSoughtTilesTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				if (suit != Suit.Zi && rank == 5)
				{
					var nonRedFives = $"555{suitString}".ToTiles();
					var redFive = $"0{suitString}".ToTile();
					yield return
					[
						new Pung(nonRedFives),
						new HashSet<Tile>([redFive]),
					];
					yield return
					[
						new Pung(nonRedFives.Take(2).Append(redFive)),
						new HashSet<Tile>(nonRedFives.Skip(2)),
					];
				}
				else
				{
					var tiles = $"{rank}{rank}{rank}{suitString}".ToTiles();
					yield return
					[
						new Pung(tiles),
						new HashSet<Tile>(tiles.Take(1)),
					];
				}
			}
		}
	}

	private static IEnumerable<object[]> UpgradePungToKongCanUpgradeWithTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				var tiles = $"{rank}{rank}{rank}{suitString}".ToTiles();
				var redFivesPossible = suit != Suit.Zi && rank == 5;
				yield return
				[
					new Pung(tiles),
					tiles.First(),
					true,
					$"a matching tile",
				];

				if (redFivesPossible)
				{
					var redFive = new Tile(suit, 0);
					yield return
					[
						new Pung(tiles),
						redFive,
						true,
						$"a matching red five",
					];
					yield return
					[
						new Pung(tiles.Take(2).Append(redFive)),
						new Tile(suit, 5),
						true,
						$"a matching non-red five",
					];
				}

				foreach (var differentSuit in Enum.GetValues<Suit>())
				{
					if (suit == differentSuit || !Tile.IsValidTile(differentSuit, rank))
					{
						continue;
					}

					var differentSuitTile = new Tile(differentSuit, rank);
					yield return
					[
						new Pung(tiles),
						differentSuitTile,
						false,
						$"a tile of same rank but different suit",
					];

					if (redFivesPossible)
					{
						var redFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles.Take(2).Append(redFive)),
							differentSuitTile,
							false,
							$"a tile of same rank but different suit with a red five in the pung",
						];
					}

					var differentSuitRedFivesPossible = suit != Suit.Zi;
					if (differentSuitRedFivesPossible)
					{
						var differentSuitRedFive = new Tile(differentSuit, 0);
						yield return
						[
							new Pung(tiles),
							differentSuitRedFive,
							false,
							$"a red five of a different suit",
						];

						if (redFivesPossible)
						{
							var redFive = new Tile(suit, 0);
							yield return
							[
								new Pung(tiles.Take(2).Append(redFive)),
								differentSuitRedFive,
								false,
								$"a red five of a different suit with a red five in the pung",
							];
						}
					}
				}

				for (int differentRank = 1; differentRank <= 9; differentRank++)
				{
					if (rank == differentRank || !Tile.IsValidTile(suit, differentRank))
					{
						continue;
					}

					var differentRankTile = new Tile(suit, differentRank);
					yield return
					[
						new Pung(tiles),
						differentRankTile,
						false,
						$"a tile of same suit but different rank",
					];

					if (redFivesPossible)
					{
						var redFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles.Take(2).Append(redFive)),
							differentRankTile,
							false,
							$"a tile of same suit but different rank with a red five in the pung",
						];
					}
					else if (suit != Suit.Zi && differentRank == 5)
					{
						var differentRankRedFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles),
							differentRankRedFive,
							false,
							$"a red five of same suit but different rank",
						];
					}
				}
			}
		}
	}

	private static IEnumerable<object[]> UpgradePungToKongNegativeTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				var tiles = $"{rank}{rank}{rank}{suitString}".ToTiles();
				var redFivesPossible = suit != Suit.Zi && rank == 5;

				foreach (var differentSuit in Enum.GetValues<Suit>())
				{
					if (suit == differentSuit || !Tile.IsValidTile(differentSuit, rank))
					{
						continue;
					}

					var differentSuitTile = new Tile(differentSuit, rank);
					yield return
					[
						new Pung(tiles),
						differentSuitTile,
						$"a tile of same rank but different suit",
					];

					if (redFivesPossible)
					{
						var redFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles.Take(2).Append(redFive)),
							differentSuitTile,
							$"a tile of same rank but different suit with a red five in the pung",
						];
					}

					var differentSuitRedFivesPossible = suit != Suit.Zi;
					if (differentSuitRedFivesPossible)
					{
						var differentSuitRedFive = new Tile(differentSuit, 0);
						yield return
						[
							new Pung(tiles),
							differentSuitRedFive,
							$"a red five of a different suit",
						];

						if (redFivesPossible)
						{
							var redFive = new Tile(suit, 0);
							yield return
							[
								new Pung(tiles.Take(2).Append(redFive)),
								differentSuitRedFive,
								$"a red five of a different suit with a red five in the pung",
							];
						}
					}
				}

				for (int differentRank = 1; differentRank <= 9; differentRank++)
				{
					if (rank == differentRank || !Tile.IsValidTile(suit, differentRank))
					{
						continue;
					}

					var differentRankTile = new Tile(suit, differentRank);
					yield return
					[
						new Pung(tiles),
						differentRankTile,
						$"a tile of same suit but different rank",
					];

					if (redFivesPossible)
					{
						var redFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles.Take(2).Append(redFive)),
							differentRankTile,
							$"a tile of same suit but different rank with a red five in the pung",
						];
					}
					else if (suit != Suit.Zi && differentRank == 5)
					{
						var differentRankRedFive = new Tile(suit, 0);
						yield return
						[
							new Pung(tiles),
							differentRankRedFive,
							$"a red five of same suit but different rank",
						];
					}
				}
			}
		}
	}

	private static IEnumerable<object[]> UpgradePungToKongPositiveTestCases()
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (int rank = 1; rank <=9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					break;
				}

				var suitString = suit.GetSuitString();
				var tiles = $"{rank}{rank}{rank}{suitString}".ToTiles();
				var redFivesPossible = suit != Suit.Zi && rank == 5;
				yield return
				[
					new Pung(tiles),
					tiles.First(),
					new Kong(tiles.Append(new Tile(suit, rank))),
					$"a matching tile",
				];

				if (redFivesPossible)
				{
					var redFive = new Tile(suit, 0);
					yield return
					[
						new Pung(tiles),
						redFive,
						new Kong(tiles.Append(redFive)),
						$"a matching red five",
					];
					yield return
					[
						new Pung(tiles.Take(2).Append(redFive)),
						new Tile(suit, 5),
						new Kong(tiles.Append(redFive)), // a bit wonky, but equivalent
						$"a matching non-red five",
					];
				}
			}
		}
	}
}
