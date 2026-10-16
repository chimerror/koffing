using System;
using GdUnit4;
using Godot;
using Koffing.Godot.UI.Elements;
using Koffing.Tiles;
using static GdUnit4.Assertions;
using static TestLoggingHelpers;

namespace Koffing.Tests;

[TestSuite]
public class TileRectTests
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

	private static readonly string _tileRectScenePath = "res://UI/Elements/TileRect.tscn";
	private static readonly string _tileRectNodePath = "TileRect";

	[TestCase]
	[RequireGodotRuntime]
	public static void TileRectRotationWorksCorrectly()
	{
		LoggingPrefix = nameof(TileRectRotationWorksCorrectly);

		var tileRect = ISceneRunner.Load(_tileRectScenePath)
			.Scene()
			.GetTree()
			.Root
			.GetNode<TileRect>(_tileRectNodePath);
		PrefixInfo("Checking that we got the tile rect...");
		AssertObject(tileRect).IsNotNull();

		PrefixInfo("Checking that tile rect starts not rotated...");
		AssertThat(tileRect.Rotated).IsFalse();
		PrefixInfo("Checking vertical tile rect size...");
		AssertThat(tileRect.Size).IsEqual(new Vector2(tileRect.TileWidth, tileRect.TileHeight));
		var verticalBase = tileRect.Position.Y + tileRect.Size.Y;

		PrefixInfo("Checking that tile can be rotated...");
		tileRect.Rotated = true;
		PrefixInfo("Checking horizontal tile rect size...");
		AssertThat(tileRect.Size).IsEqual(new Vector2(tileRect.TileHeight, tileRect.TileWidth));
		var horizontalBase = tileRect.Position.Y + tileRect.Size.Y;
		PrefixInfo("Checking that horizontal base is the same as vertical base...");
		AssertThat(horizontalBase).IsEqual(verticalBase);
	}

	[TestCase]
	[RequireGodotRuntime]
	public static void TileRectRankIndicatorWorksCorrectly()
	{
		LoggingPrefix = nameof(TileRectRankIndicatorWorksCorrectly);

		var tileRect = ISceneRunner.Load(_tileRectScenePath)
			.Scene()
			.GetTree()
			.Root
			.GetNode<TileRect>(_tileRectNodePath);
		PrefixInfo("Checking that we got the tile rect...");
		AssertObject(tileRect).IsNotNull();

		PrefixInfo("Checking that tile rect starts face up...");
		AssertThat(tileRect.FaceUp).IsTrue();

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 0; rank <= 9; rank++)
			{
				if (!Tile.IsValidTile(suit, rank))
				{
					continue;
				}

				PrefixInfo($"Setting tile rect to {rank} {suit}...");
				tileRect.Rank = rank; // Have to set rank first to ensure we keep the tile valid
				tileRect.Suit = suit;

				PrefixInfo("Setting tile rect to face-up vertical orientation with shown rank indicator...");
				tileRect.Rotated = false;
				tileRect.FaceUp = true;
				tileRect.ShowRankIndicator = true;

				var expectedRankIndicatorText = rank == 0 ? "5" : rank.ToString();
				if (suit == Suit.Zi)
				{
					// Copied over from the implementation, but a case where I am accepting it.
					expectedRankIndicatorText = rank switch
					{
						1 => "E",
						2 => "S",
						3 => "W",
						4 => "N",
						5 => "Wh",
						6 => "G",
						7 => "R",
						_ => throw new InvalidOperationException($"Unexpected Zi rank passed in: {rank}"),
					};
				}
				PrefixInfo($"Checking vertical rank indicator text for {rank} {suit} is \"{expectedRankIndicatorText}\"...");
				AssertThat(tileRect.CurrentRankIndicator.Text).IsEqual(expectedRankIndicatorText);

				var expectedRankIndicatorColor = rank == 0 ?
					tileRect.RedFiveRankIndicatorColor :
					tileRect.NormalRankIndicatorColor;
				var actualVerticalRankIndicatorColor = tileRect.CurrentRankIndicator.GetThemeColor("font_color");
				PrefixInfo($"Checking vertical rank indicator color...");
				AssertThat(actualVerticalRankIndicatorColor).IsEqual(expectedRankIndicatorColor);

				PrefixInfo($"Checking that vertical rank indicator is not visible when face down...");
				tileRect.FaceUp = false;
				AssertThat(tileRect.CurrentRankIndicator.Visible).IsFalse();

				PrefixInfo($"Checking that vertical rank indicator is not visible when disabled...");
				tileRect.FaceUp = true;
				tileRect.ShowRankIndicator = false;
				AssertThat(tileRect.CurrentRankIndicator.Visible).IsFalse();

				PrefixInfo("Setting tile rect to be face-up horizontal orientation with shown rank indicator...");
				tileRect.ShowRankIndicator = true;
				tileRect.Rotated = true;

				PrefixInfo($"Checking horizontal rank indicator text for {rank} {suit} is \"{expectedRankIndicatorText}\"...");
				AssertThat(tileRect.CurrentRankIndicator.Text).IsEqual(expectedRankIndicatorText);

				var actualHorizontalRankIndicatorColor = tileRect.CurrentRankIndicator.GetThemeColor("font_color");
				PrefixInfo($"Checking horizontal rank indicator color...");
				AssertThat(actualHorizontalRankIndicatorColor).IsEqual(expectedRankIndicatorColor);

				PrefixInfo($"Checking that horizontal rank indicator is not visible when face down...");
				tileRect.FaceUp = false;
				AssertThat(tileRect.CurrentRankIndicator.Visible).IsFalse();

				PrefixInfo($"Checking that horizontal rank indicator is not visible when disabled...");
				tileRect.FaceUp = true;
				tileRect.ShowRankIndicator = false;
				AssertThat(tileRect.CurrentRankIndicator.Visible).IsFalse();
			}
		}
	}
}