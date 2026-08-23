using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
[RequireGodotRuntime]
public class BlockTests
{
	[TestCase]
	[DataPoint(nameof(PungGetPossibleForTileTestCases))]
	public static void PungGetPossibleForTileIsCorrect(
		Tile tile,
		IEnumerable<Tile> otherTiles,
		List<MadeBlockContext> expectedOutput)
	{
		var actualOutput = Pung.GetPossibleForTile(tile, otherTiles).ToList();
		AssertArray(actualOutput).ContainsExactly(expectedOutput);
	}

	private static IEnumerable<object[]> PungGetPossibleForTileTestCases()
	{
		// when there is only one matching tile
		yield return ["5z".ToTile(true), "2p3s11m5z".ToTiles(true), new List<MadeBlockContext>()];

		// when there are no matching tiles
		yield return ["5z".ToTile(true), "2p3s11m".ToTiles(true), new List<MadeBlockContext>()];

		// when there are more than enough matching pin tiles
		yield return [
			"7p".ToTile(true),
			"777p3s11m66z".ToTiles(true),
			new List<MadeBlockContext>() {
				new(new Pung("777p".ToTiles(true)), "7p3s11m66z".ToTiles(true)),
			}];
	}
}