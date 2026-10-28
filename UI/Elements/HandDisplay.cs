using System.Collections.Generic;
using Godot;

namespace Koffing.Godot.UI.Elements;

public partial class HandDisplay : Control
{
	private TileRect _templateTileRect;
	private TileRect _peiDoraTemplateTileRect;
	private HBoxContainer _handHBox;
	private HBoxContainer _drawnTileHBox;
	private HBoxContainer _meldedTilesHBox;
	private HBoxContainer _peiDoraHBox;
	private TileRect _drawnTileRect;
	private readonly List<TileRect> _handTileRects = [];
	private readonly List<TileRect> _meldedTileRects = [];
	private readonly List<TileRect> _peiDoraTileRects = [];

	// UNTESTED: When we get to being able to add and update the tiles in this, it'll probably be worth testing that
	// they end up in the correct HBoxes.
	public override void _Ready()
	{
		_templateTileRect = GetNode<TileRect>("%TemplateTileRect");
		_peiDoraTemplateTileRect = GetNode<TileRect>("%PeiDoraTemplateTileRect");
		_handHBox = GetNode<HBoxContainer>("%HandHBox");
		_drawnTileHBox = GetNode<HBoxContainer>("%DrawnTileHBox");
		_meldedTilesHBox = GetNode<HBoxContainer>("%MeldedTilesHBox");
		_peiDoraHBox = GetNode<HBoxContainer>("%PeiDoraHBox");

		// TODO: Replace this testing code with code to fill this in based on properties with tiles set.
		if (this.IsRootNode())
		{
			GD.Print("Adding testing tiles to HandDisplay!");
			for (var i = 0; i < 10; i++)
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
				if (i == 1)
				{
					tileRect.Rotated = true;
				}
				_meldedTileRects.Add(tileRect);
				_meldedTilesHBox.AddChild(tileRect);
			}

			for (var i = 0; i < 4; i++)
			{
				var tileRect = InstantiateTileRect(_peiDoraTemplateTileRect);
				_peiDoraTileRects.Add(tileRect);
				_peiDoraHBox.AddChild(tileRect);
			}
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

		foreach (var tileRect in _peiDoraTileRects)
		{
			_peiDoraHBox.RemoveChild(tileRect);
			tileRect.QueueFree();
		}
		_peiDoraTileRects.Clear();
	}

	private TileRect InstantiateTileRect(TileRect templateTileRect = null)
	{
		templateTileRect ??= _templateTileRect;
		var tileRect = (TileRect)templateTileRect.Duplicate();
		tileRect.Visible = true;
		return tileRect;
	}
}