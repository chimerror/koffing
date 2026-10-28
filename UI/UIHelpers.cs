using Godot;

namespace Koffing.Godot.UI;

public static class UIHelpers
{
	public static bool IsRootNode(this Node node)
	{
		return node.GetTree().Root.GetChild(0) == node;
	}
}