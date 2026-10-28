using System.Collections.Generic;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Players;

/// <summary>
/// A representation of the current state of a player in a <see cref="Match"/>, including their points, current hand,
/// drawn tile, melds, pei dora, and discards.
/// </summary>
public class PlayerState
{
	/// <summary>
	/// The number of points the player currently has.
	/// </summary>
	/// <value>The number of points the player currently has.</value>
	public int Points { get; set; }

	/// <summary>
	/// The <see cref="Tile"/>s the player currently has in their hand.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of <see cref="Tile"/>s the player currently has in their hand.</value>
	public List<Tile> Hand { get; set; } = [];

	/// <summary>
	/// The <see cref="Tile"/> the player has just drawn.
	/// </summary>
	/// <value>
	/// The <see cref="Tile"/> the player has just drawn, or <see langword="null"/> if they have not done so at this
	/// time.
	/// </value>
	public Tile DrawnTile { get; set; }

	/// <summary>
	/// The revealed <see cref="Meld"/>s that the player has made.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of revealed <see cref="Meld"/>s the player currently has.</value>
	// TODOTODO: Add PerformMeld method.
	public List<Meld> MadeMelds { get; set; } = [];

	/// <summary>
	/// The claimed pei dora that the player has made.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of pei dora the player has made.</value>
	// TODOTODO: Add PerformPeiDoraClaim method.
	public List<Tile> PeiDora { get; set; } = [];

	/// <summary>
	/// The <see cref="Tile"/>s the player has discarded.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of <see cref="Tile"/>s the player has discarded.</value>
	public List<Tile> Discards { get; set; } = [];
}