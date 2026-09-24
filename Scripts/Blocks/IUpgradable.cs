using System.Collections.Generic;

// TODO: I am questioning if this interface and its cousin IDowngradable is worth doing. I am going to pivot over to
// doing match setup, which I am hoping will make that question as far as others clearer. Based on that, either remove
// these interfaces, or finish implementing them for Pair and the Waits.
public interface IUpgradable<T>
{
	HashSet<Tile> SoughtTiles { get; }
	bool CanUpgradeWith(Tile tile);
	T Upgrade(Tile tile);
}