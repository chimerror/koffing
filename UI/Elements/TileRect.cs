using System;
using Godot;
using Koffing.Tiles;

namespace Koffing.Godot.UI.Elements;

[Tool]
public partial class TileRect : TextureRect
{
	private Label _verticalRankIndicator;
	private Label _horizontalRankIndicator;
	private bool _showRankIndicator = false;

	// Have to set FaceUp and Rotated to false because properties will not update by default upon loading when set to
	// false.
	private Tile _tile = new()
	{
		FaceUp = false,
		Rotated = false,
	};

	public Tile Tile
	{
		get => _tile;
		set
		{
			var needUpdate = _tile != value;
			_tile = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	[Export]
	public Color NormalRankIndicatorColor { get; set; } = new Color("red");

	[Export]
	public Color RedFiveRankIndicatorColor { get; set; } = new Color("black");

	[Export]
	public float TileWidth { get; set; } = 190f;

	[Export]
	public float TileHeight { get; set; } = 240f;

	[Export]
	public int RankIndicatorFontSize { get; set; } = 32;

	[Export]
	public int RankIndicatorFontShadowSize { get; set; } = 14;

	[Export]
	public Vector2 RankIndicatorSize { get; set; } = new(32f, 45f);

	[Export]
	public Vector2 VerticalRankIndicatorPosition { get; set; } = new(118f, 40f);

	[Export]
	public Vector2 HorizontalRankIndicatorPosition { get; set; } = new(0f, 72f);

	[Export]
	public Vector2 WestRankIndicatorOffset = new(-6f, -6f);

	[Export]
	public Vector2 WhiteRankIndicatorOffset = new(-22f, -22f);

	[Export]
	public Suit Suit
	{
		get => _tile.Suit;
		set
		{
			var needUpdate = value != _tile.Suit;
			_tile.Suit = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	[Export(PropertyHint.Range, "0,9,")]
	public int Rank
	{
		get => _tile.Rank;
		set
		{
			if (value < 0 || value > 9 || (_tile.Suit == Suit.Zi && (value == 0 || value > 7)))
			{
				GD.PrintErr($"Set rank to invalid value {value} for suit {_tile.Suit}!");
			}
			var needUpdate = value != _tile.Rank;
			_tile.Rank = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	[Export]
	public bool FaceUp
	{
		get => _tile.FaceUp;
		set
		{
			var needUpdate = value != _tile.FaceUp;
			_tile.FaceUp = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	[Export]
	public bool Rotated
	{
		get => _tile.Rotated;
		set
		{
			var needUpdate = value != _tile.Rotated;
			_tile.Rotated = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	[Export]
	public bool ShowRankIndicator
	{
		get => _showRankIndicator;
		set
		{
			var needUpdate = value != _showRankIndicator;
			_showRankIndicator = value;
			if (needUpdate)
			{
				UpdateTileRect();
			}
		}
	}

	// Internal for the sake of testing.
	internal Label CurrentRankIndicator => Rotated ? _horizontalRankIndicator : _verticalRankIndicator;

	public override string ToString()
	{
		return nameof(TileRect) + ":" + _tile?.NotationFromTile();
	}

	private void UpdateTileRect()
	{
		var previousRankIndicator = Rotated ? _verticalRankIndicator : _horizontalRankIndicator;
		if (previousRankIndicator != null)
		{
			previousRankIndicator.Visible = false;
		}

		if (CurrentRankIndicator != null)
		{
			if (Suit != Suit.Zi)
			{
				var indicatorValue = Rank == 0 ? 5 : Rank;
				CurrentRankIndicator.Text = $"{indicatorValue}";
			}
			else
			{
				CurrentRankIndicator.Text = Rank switch
				{
					1 => "E",
					2 => "S",
					3 => "W",
					4 => "N",
					5 => "Wh",
					6 => "G",
					7 => "R",
					_ => throw new InvalidOperationException($"Unexpected Zi rank passed in: {Rank}"),
				};
			}

			CurrentRankIndicator.Visible = FaceUp && _showRankIndicator;
			CurrentRankIndicator.AddThemeColorOverride(
				"font_color",
				Rank == 0 ? RedFiveRankIndicatorColor : NormalRankIndicatorColor);

			// Nudge the rank indicator to make up for how much longer "W" and "Wh are
			if (Suit == Suit.Zi && (Rank == 3 || Rank == 5))
			{
				var offsetToApply = Rank == 3 ? WestRankIndicatorOffset : WhiteRankIndicatorOffset;
				var offsetToNudgeWith = Rotated ? offsetToApply.Y : offsetToApply.X;
				CurrentRankIndicator.OffsetTransformPosition = new(offsetToNudgeWith, 0f);
				CurrentRankIndicator.OffsetTransformEnabled = true;
			}
			else
			{
				CurrentRankIndicator.OffsetTransformEnabled = false;
			}
		}

		var spriteDirectory = Rotated ? "Horizontal" : "Vertical";
		if (FaceUp)
		{
			Texture = GD.Load<CompressedTexture2D>($"res://Sprites/Tiles/Pink/{spriteDirectory}/{Rank}{Suit.ToString().ToLower()}.png");
		}
		else
		{
			Texture = GD.Load<CompressedTexture2D>($"res://Sprites/Tiles/Pink/{spriteDirectory}/back.png");
		}

		// The adjustment of the tile position has to come after setting the rank indicator position, because otherwise,
		// the rank indicator position will not follow along with the new tile position.
		if (Rotated)
		{
			CustomMaximumSize = new(TileHeight, TileWidth);

			// TODO: This logic made sense when thinking about a rotated tile in the hand, but when declaring riichi,
			// MajSoul places the rotated tile in the center of the vertical neighbor (so 0.5 * TileWidth), while the
			// diagram in the WRC rules places it at the top of its neighbor (so 0 * TileWidth). This variation is
			// enough to make me convinced it doesn't really matter if we keep it at the bottom, but consider if it
			// would be clearer to choose one of the other options.
			// When rotated, move the tile down so that its bottom is the same place as the bottom of vertical tiles.
			OffsetTransformPosition = new(0f, TileHeight - TileWidth);
			OffsetTransformVisualOnly = false;
			OffsetTransformEnabled = true;
		}
		else
		{
			CustomMaximumSize = new(TileWidth, TileHeight);
			OffsetTransformEnabled = false;
		}
	}

	public override void _Ready()
	{
		_verticalRankIndicator = GetNode<Label>("%VerticalRankIndicator");
		SetUpRankIndicator(_verticalRankIndicator, VerticalRankIndicatorPosition);

		_horizontalRankIndicator = GetNode<Label>("%HorizontalRankIndicator");
		SetUpRankIndicator(_horizontalRankIndicator, HorizontalRankIndicatorPosition);

		UpdateTileRect();
	}

	private void SetUpRankIndicator(Label rankIndicatorLabel, Vector2 rankIndicatorPosition)
	{
		rankIndicatorLabel.AddThemeFontSizeOverride("font_size", RankIndicatorFontSize);
		rankIndicatorLabel.AddThemeConstantOverride("shadow_outline_size", RankIndicatorFontShadowSize);
		rankIndicatorLabel.CustomMinimumSize = new(RankIndicatorFontSize, 0f);
		rankIndicatorLabel.Size = RankIndicatorSize;
		rankIndicatorLabel.Position = rankIndicatorPosition;
	}
}