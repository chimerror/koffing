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
		AssertArray(actualOutput).ContainsExactlyInAnyOrder(expectedOutput);
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
			}
		];

		// when there are enough matching sou tiles
		yield return [
			"4s".ToAutoFreeTile(),
			"067p1234456s11m66z".ToAutoFreeTiles(),
			new List<MadeBlockContext>() {
				new(new Pung("444s".ToAutoFreeTiles()), "067p12356s11m66z".ToAutoFreeTiles()),
			}
		];

		// when there are enough matching man tiles
		yield return [
			"1m".ToAutoFreeTile(),
			"2p3s11m66z".ToAutoFreeTiles(),
			new List<MadeBlockContext>() {
				new(new Pung("111m".ToAutoFreeTiles()), "2p3s66z".ToAutoFreeTiles()),
			}
		];

		// when there are enough matching zi tiles
		yield return [
			"5z".ToAutoFreeTile(),
			"2p3s11m55z".ToAutoFreeTiles(),
			new List<MadeBlockContext>() {
				new(new Pung("555z".ToAutoFreeTiles()), "2p3s11m".ToAutoFreeTiles()),
			}
		];

		// when given red five and there are enough matching tiles
		yield return [
			"0m".ToAutoFreeTile(),
			"2p3s55m77z".ToAutoFreeTiles(),
			new List<MadeBlockContext>() {
				new(new Pung("055m".ToAutoFreeTiles()), "2p3s77z".ToAutoFreeTiles()),
			}
		];

		// when given non-red five and there are more than enough matching tiles with a red five
		yield return [
			"5s".ToAutoFreeTile(),
			"067p12344550s11m66z".ToAutoFreeTiles(),
			new List<MadeBlockContext>() {
				new(new Pung("555s".ToAutoFreeTiles()), "067p12344s11m66z0s".ToAutoFreeTiles()),
				new(new Pung("550s".ToAutoFreeTiles()), "067p12344s11m66z5s".ToAutoFreeTiles()),
			}
		];
	}
}