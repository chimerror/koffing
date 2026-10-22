using System.Collections.Generic;
using Godot;

namespace Koffing.Godot.UI.Elements;

public partial class HandDisplay : HBoxContainer
{
	private TileRect _templateTileRect;
	private HBoxContainer _handHBox;
	private HBoxContainer _drawnTileHBox;
	private HBoxContainer _meldedTilesHBox;
	private TileRect _drawnTileRect;
	private readonly List<TileRect> _handTileRects = [];
	private readonly List<TileRect> _meldedTileRects = [];

	public override void _Ready()
	{
		_templateTileRect = GetNode<TileRect>("%TemplateTileRect");
		_handHBox = GetNode<HBoxContainer>("%HandHBox");
		_drawnTileHBox = GetNode<HBoxContainer>("%DrawnTileHBox");
		_meldedTilesHBox = GetNode<HBoxContainer>("%MeldedTilesHBox");

		// TODO: Replace this testing code with code to fill this in based on properties with tiles set.
		for (var i = 0; i < 13; i++)
		{
			var tileRect = InstantiateTileRect();
			_handTileRects.Add(tileRect);
			_handHBox.AddChild(tileRect);
		}

		_drawnTileRect = InstantiateTileRect();
		_drawnTileHBox.AddChild(_drawnTileRect);

		for (var i = 0; i < 3; i++)
		{
			var tileRect = InstantiateTileRect();
			_meldedTileRects.Add(tileRect);
			_meldedTilesHBox.AddChild(tileRect);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		foreach (var tileRect in _handTileRects)
		{
			_handHBox.RemoveChild(tileRect);
			tileRect.QueueFree();
		}
		_handTileRects.Clear();

		_drawnTileHBox.RemoveChild(_drawnTileRect);
		_drawnTileRect.QueueFree();
		_drawnTileRect = null;

		foreach (var tileRect in _meldedTileRects)
		{
			_meldedTilesHBox.RemoveChild(tileRect);
			tileRect.QueueFree();
		}
		_meldedTileRects.Clear();
	}

	private TileRect InstantiateTileRect()
	{
		var tileRect = (TileRect)_templateTileRect.Duplicate();
		tileRect.Visible = true;
		return tileRect;
	}
}