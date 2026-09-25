using Godot;
using System.Collections.Generic;

[Tool]
public partial class WallDisplay : Node2D
{
	private ulong _seed = 0ul;
	private readonly GodotRandomNumberGenerator _rng = new();
	private PlayerCount _playerCount = PlayerCount.Four;
	private bool _hasRedFives = true;
	private Wall _wall;
	private readonly List<TileSprite> _tileSprites = [];

	private bool _listPositions = false;

	[Export]
	public PackedScene TileScene { get; set; } = ResourceLoader.Load<PackedScene>("res://Scenes/TileSprite.tscn");

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

		_wall = new Wall(_rng, _playerCount, _hasRedFives);
		foreach (var tile in _wall.Tiles)
		{
			var tileSprite = TileScene.Instantiate<TileSprite>();
			tileSprite.Tile = tile;
			_tileSprites.Add(tileSprite);
			AddChild(tileSprite);
		}

		_listPositions = true;
	}

	private void FreeTiles()
	{
		foreach (var tileSprite in _tileSprites)
		{
			RemoveChild(tileSprite);
			tileSprite.QueueFree();
		}
		_tileSprites.Clear();
	}

	public WallDisplay()
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

		foreach (var tileSprite in _tileSprites)
		{
			tileSprite.Position = new Vector2(currentX, currentY);
			tileSprite.ZIndex = currentZIndex;

			if (_listPositions)
			{
				GD.Print($"Positioning {tileSprite.Rank} of {tileSprite.Suit} at ({tileSprite.Position.X}, {tileSprite.Position.Y}, {tileSprite.ZIndex})");
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
