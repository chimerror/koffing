using Godot;
using Koffing.Tiles;

namespace Koffing.Godot.UI.Elements;

[Tool]
public partial class TileRect : TextureRect
{
	// Have to set FaceUp to false because the FaceUp property will not update by default upon loading when set to
	// false.
	private Tile _tile = new() { FaceUp = false };

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
	public override string ToString()
	{
		return nameof(TileRect) + ":" + _tile.NotationFromTile();
	}

	private void UpdateTileRect()
	{
		if (FaceUp)
		{
			Texture = GD.Load<CompressedTexture2D>($"res://Sprites/Tiles/{Rank}{Suit.ToString().ToLower()}.png");
		}
		else
		{
			Texture = GD.Load<CompressedTexture2D>($"res://Sprites/Tiles/back.png");
		}
	}

	public override void _Ready()
	{
		UpdateTileRect();
	}
}
