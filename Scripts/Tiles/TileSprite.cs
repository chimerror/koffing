using Godot;

[Tool]
public partial class TileSprite : Sprite2D
{
	private readonly Tile _tile = new();

	public Tile Tile => _tile;

	[Export]
	public Suit Suit
	{
		get => _tile.Suit;
		set
		{
			var needUpdate = value != _tile.Suit;
			_tile.Suit = value;
			if (needUpdate) {
				UpdateTileSprite();
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
			if (needUpdate) {
				UpdateTileSprite();
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
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}
	public override string ToString()
	{
		return nameof(TileSprite) + ":" + _tile.NotationFromTile();
	}

	private void UpdateTileSprite()
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
		UpdateTileSprite();
	}
}
