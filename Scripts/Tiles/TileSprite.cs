using Godot;

[Tool]
public partial class TileSprite : Sprite2D
{
	[Export]
	public Tile Tile { get; set; } = new Tile(Suit.Man, 1);

	[Export]
	public Suit Suit
	{
		get => Tile.Suit;
		set
		{
			var needUpdate = value != Tile.Suit;
			Tile.Suit = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export(PropertyHint.Range, "0,9,")]
	public int Rank
	{
		get => Tile.Rank;
		set
		{
			if (value < 0 || value > 9 || (Tile.Suit == Suit.Zi && (value == 0 || value > 7)))
			{
				GD.PrintErr($"Set rank to invalid value {value} for suit {Tile.Suit}!");
			}
			var needUpdate = value != Tile.Rank;
			Tile.Rank = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export]
	public bool FaceUp
	{
		get => Tile.FaceUp;
		set
		{
			var needUpdate = value != Tile.FaceUp;
			Tile.FaceUp = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}
	public override string ToString()
	{
		return nameof(TileSprite) + ":" + this.Tile.NotationFromTile();
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
