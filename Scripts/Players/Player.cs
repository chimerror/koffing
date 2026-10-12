using System.Collections.Generic;
using Koffing.Tiles;

namespace Koffing.Players;

/// <summary>
/// Base class for a player in a <see cref="Matches.Match"/>.
/// </summary>
public class Player
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

	// TODD: Have players be able to respond to match situations. This will probably be an interface.
}