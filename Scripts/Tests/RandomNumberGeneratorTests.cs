using System.Collections.Generic;
using System.Linq;
using GdUnit4;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

[TestSuite]
public class RandomNumberGeneratorTests
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

	private static readonly string[] _windTileStrings = ["1z", "2z", "3z", "4z"];
	private static readonly string[] _godotExpectedShuffleTileStrings = ["3z", "4z", "2z", "1z"];
	private static readonly string[] _systemExpectedShuffleTileStrings = ["3z", "4z", "1z", "2z"];

	[TestCase]
	[RequireGodotRuntime]
	public static void GodotRandomNumberGeneratorCanBeUsed()
	{
		LoggingPrefix = nameof(GodotRandomNumberGeneratorCanBeUsed);

		PrefixInfo("Checking that Godot RNG can be constructed...");
		var rng = new GodotRandomNumberGenerator();
		PrefixInfo("Checking that Godot RNG can be randomized...");
		rng.Randomize();
		PrefixInfo("Checking that Godot RNG can return ranged integer...");
		AssertThat(rng.GetIntegerInRange(-13, 13)).IsBetween(-13, 13);
		PrefixInfo("Checking that Godot RNG can have seed set...");
		rng.Seed = 13;
		AssertThat(rng.GetIntegerInRange(-13, 13)).IsEqual(-4);
		PrefixInfo("Checking that Godot RNG can shuffle tiles...");
		List<Tile> tiles = [.. _windTileStrings.Select(s => s.ToTile())];
		var shuffledTiles = rng.Shuffle(tiles);
		List<Tile> expectedTiles = [.. _godotExpectedShuffleTileStrings.Select(s => s.ToTile())];
		AssertArray(expectedTiles).ContainsExactly(shuffledTiles);
	}

	[TestCase]
	public static void SystemRandomNumberGeneratorCanBeUsed()
	{
		LoggingPrefix = nameof(SystemRandomNumberGeneratorCanBeUsed);

		PrefixInfo("Checking that System RNG can be constructed...");
		var rng = new SystemRandomNumberGenerator();
		PrefixInfo("Checking that System RNG can be randomized...");
		rng.Randomize();
		PrefixInfo("Checking that System RNG can return ranged integer...");
		AssertThat(rng.GetIntegerInRange(-13, 13)).IsBetween(-13, 13);
		PrefixInfo("Checking that System RNG can have seed set...");
		rng.Seed = 13;
		AssertThat(rng.GetIntegerInRange(-13, 13)).IsEqual(0);
		PrefixInfo("Checking that System RNG can shuffle tiles...");
		List<Tile> tiles = [.. _windTileStrings.Select(s => s.ToTile())];
		var shuffledTiles = rng.Shuffle(tiles);
		List<Tile> expectedTiles = [.. _systemExpectedShuffleTileStrings.Select(s => s.ToTile())];
		AssertArray(expectedTiles).ContainsExactly(shuffledTiles);
	}
}