using System;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class WaitTests
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
	public static void CanCreateOrphan()
	{
		LoggingPrefix = nameof(CanCreateOrphan);

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!Tile.IsValidTile(suit, rank))
				{
					continue;
				}

				var tile = new Tile(suit, rank);
				PrefixInfo($"Checking that we can create an orphan from tile \"{tile}\"...");
				var orphan = new Orphan(tile);
				AssertThat(orphan.Tiles.Single()).IsEqual(tile);
			}
		}
	}
}