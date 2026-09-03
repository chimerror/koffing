using Godot;

public partial class TileResource : Resource
{
	private readonly Tile _tile = new();

	public Tile Tile => _tile;

	[Export]
	public Suit Suit
	{
		get => _tile.Suit;
		set => _tile.Suit = value;
	}

	[Export]
	public int Rank
	{
		get => _tile.Rank;
		set => _tile.Rank = value;
	}

	[Export]
	public bool FaceUp
	{
		get => _tile.FaceUp;
		set => _tile.FaceUp = value;
	}
}