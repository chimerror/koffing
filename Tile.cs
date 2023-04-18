using Godot;
using System;

[Tool]
public partial class Tile : Node2D
{
	private Suit _suit = Suit.Man;
	private int _rank = 1;

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

	private void UpdateTileSprite()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = GD.Load<CompressedTexture2D>($"res://Sprites/Tiles/{Rank}{Suit.ToString().ToLower()}.png");
	}

	public override void _Ready()
	{
		UpdateTileSprite();
	}

	public override void _Process(double delta)
	{
	}
}