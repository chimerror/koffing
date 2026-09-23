using System.Collections.Generic;

public interface IUpgradable<T>
{
	HashSet<Tile> SoughtTiles { get; }
	bool CanUpgradeWith(Tile tile);
	T Upgrade(Tile tile);
}