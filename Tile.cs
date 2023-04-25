using Godot;
using System;

[Tool]
public partial class Tile : Sprite2D
{
	private Suit _suit = Suit.Man;
	private int _rank = 1;
	private bool _faceUp = true;

	[Export]
	public Suit Suit
	{
		get => _suit;
		set
		{
			var needUpdate = value != _suit;
			_suit = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export(PropertyHint.Range, "0,9,")]
	public int Rank
	{
		get => _rank;
		set
		{
			if (value < 0 || value > 9 || (_suit == Suit.Zi && (value == 0 || value > 7)))
			{
				GD.PrintErr($"Set rank to invalid value {value} for suit {_suit}!");
			}
			var needUpdate = value != _rank;
			_rank = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	[Export]
	public bool FaceUp
	{
		get => _faceUp;
		set
		{
			var needUpdate = value != _faceUp;
			_faceUp = value;
			if (needUpdate) {
				UpdateTileSprite();
			}
		}
	}

	public int RawRank
	{
		get => _rank == 0 ? 5 : _rank;
	}

	public override bool Equals(Object obj)
	{
		if (obj == null)
		{
			return false;
		}

		var objTile = obj as Tile;
		if (objTile == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == objTile.Suit) && (Rank == objTile.Rank);
	}

	public bool Equals(Tile otherTile)
	{
		if (otherTile == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == otherTile.Suit) && (Rank == otherTile.Rank);
	}


	public override int GetHashCode()
	{
		var suitInt = (int)Suit;
		var rankInt = Rank + 1; // Increment so the range is 1-10 instead of 0-9
		return suitInt ^ rankInt;
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
