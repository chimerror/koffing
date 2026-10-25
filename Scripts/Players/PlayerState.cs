using System.Collections.Generic;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Players;

/// <summary>
/// A representation of the current state of a player in a <see cref="Match"/>, including their points, current hand,
/// drawn tile, and melds.
/// </summary>
public class PlayerState
{
	/// <summary>
	/// The number of points the player currently has.
	/// </summary>
	/// <value>The number of points the player currently has.</value>
	// TODO: Might want to move these to a PlayerState class or something like that eventually, but for now, this works.
	public int Points { get; set; }

	/// <summary>
	/// The <see cref="Tile"/>s the player currently has in their hand.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of <see cref="Tile"/>s the player currently has in their hand.</value>
	public List<Tile> Hand { get; set; }

	public Tile DrawnTile { get; set; }

	public List<Meld> MadeMelds { get; set; }

}