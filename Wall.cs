using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Wall : Node2D
{
	private ulong _seed = 0ul;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	private PlayerCount _playerCount = PlayerCount.Four;
	private bool _hasRedFives = true;
	private List<Tile> _tiles = new List<Tile>();

	private bool _listPositions = false;

	[Export]
	public PackedScene TileScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Tile.tscn");

	[Export]
	public ulong Seed
	{
		get => _seed;
		set
		{
			var needUpdate = value != _seed;
			_seed = value;
			if (needUpdate) {
				UpdateWall();
			}
		}
	}

	[Export]
	public PlayerCount PlayerCount
	{
		get => _playerCount;
		set
		{
			var needUpdate = value != _playerCount;
			_playerCount = value;
			if (needUpdate) {
				UpdateWall();
			}
		}
	}

	[Export]
	public bool HasRedFives
	{
		get => _hasRedFives;
		set
		{
			var needUpdate = value != _hasRedFives;
			_hasRedFives = value;
			if (needUpdate) {
				UpdateWall();
			}
		}
	}

	private void UpdateWall()
	{
		GD.Print("Updating Wall...");
		FreeTiles();

		if (_seed != 0ul)
		{
			_rng.Seed = _seed;
		}
		else
		{
			_rng.Randomize();
			GD.Print($"RNG Set to {_rng.Seed}");
		}

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 1; rank <= 9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					continue;
				}
				else if (_playerCount == PlayerCount.Three && suit == Suit.Man && !(rank == 1 || rank == 9))
				{
					continue;
				}

				for (var instance = 0; instance < 4; instance++)
				{
					var adjustedRank = rank;
					if (_hasRedFives && suit != Suit.Zi && rank == 5 && instance == 0)
					{
						adjustedRank = 0;
					}

					var tile = TileScene.Instantiate<Tile>();
					tile.Suit = suit;
					tile.Rank = adjustedRank;
					/* tile.FaceUp = false; */
					_tiles.Add(tile);
				}
			}
		}

		_tiles = _rng.Shuffle(_tiles);
		foreach (var tile in _tiles)
		{
			AddChild(tile);
		}
		_listPositions = true;
	}

	private void FreeTiles()
	{
		foreach (var tile in _tiles)
		{
			RemoveChild(tile);
			tile.QueueFree();
		}
		_tiles.Clear();
	}

	public Wall()
	{
		UpdateWall();
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var currentX = -1050f;
		var currentY = 0f;
		var currentTileInRow = 1;
		var currentZIndex = 0;

		foreach (var tile in _tiles)
		{
			tile.Position = new Vector2(currentX, currentY);
			tile.ZIndex = currentZIndex;

			if (_listPositions)
			{
				GD.Print($"Positioning {tile.Rank} of {tile.Suit} at ({tile.Position.X}, {tile.Position.Y}, {tile.ZIndex})");
			}
			currentX += 150f;
			currentTileInRow++;
			if (currentTileInRow > 15)
			{
				currentX = -1050f;
				currentY -= 200f;
				currentZIndex++;
				currentTileInRow = 1;
			}
		}
		_listPositions = false;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		FreeTiles();
	}
}
