using System.Collections.Generic;
using Godot;

namespace Koffing.Godot.UI.Elements;

public partial class DiscardsDisplay : HFlowContainer
{
	private TileRect _templateTileRect;
	private readonly List<TileRect> _tileRects = [];

	public async override void _Ready()
	{
		_templateTileRect = GetNode<TileRect>("%TemplateTileRect");

		if (this.IsRootNode())
		{
			GD.Print("Adding testing tiles for DiscardsDisplay!");
			for (var i = 0; i < 19; i++)
			{
				var tileRect = (TileRect)_templateTileRect.Duplicate();
				tileRect.Visible = true;

				if (i == 13)
				{
					tileRect.Rotated = true;
				}

				AddChild(tileRect);
				_tileRects.Add(tileRect);
			}
		}
	}

	// UNTESTED: There should really be a test on this, but wait until we implement being able to add a tile to the
	// discards.
	public override void _Notification(int what)
	{
		if (what == NotificationSortChildren)
		{
			var currentColumn = 0;
			var currentZIndex = 0;
			foreach (var node in GetChildren())
			{
				if (node is not TileRect tileRect)
				{
					continue;
				}

				if (tileRect == _templateTileRect)
				{
					continue;
				}

				tileRect.ZIndex = currentZIndex;

				currentColumn++;
				if (currentColumn >= 6)
				{
					currentColumn = 0;
					currentZIndex--;
				}
			}
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		foreach (var tileRect in _tileRects)
		{
			RemoveChild(tileRect);
			tileRect.QueueFree();
		}
		_tileRects.Clear();
	}
}