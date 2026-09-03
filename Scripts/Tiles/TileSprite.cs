using Godot;

[Tool]
public partial class TileSprite : Sprite2D
{
	[Export]
	public TileResource TileResource { get; set; } = new TileResource();

	[Export]
	public Suit Suit
	{
		get => TileResource.Suit;
		set
		{
			var needUpdate = value != TileResource.Suit;
			TileResource.Suit = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export(PropertyHint.Range, "0,9,")]
	public int Rank
	{
		get => TileResource.Rank;
		set
		{
			if (value < 0 || value > 9 || (TileResource.Suit == Suit.Zi && (value == 0 || value > 7)))
			{
				GD.PrintErr($"Set rank to invalid value {value} for suit {TileResource.Suit}!");
			}
			var needUpdate = value != TileResource.Rank;
			TileResource.Rank = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export]
	public bool FaceUp
	{
		get => TileResource.FaceUp;
		set
		{
			var needUpdate = value != TileResource.FaceUp;
			TileResource.FaceUp = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}
	public override string ToString()
	{
		return nameof(TileSprite) + ":" + TileResource.Tile.NotationFromTile();
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
