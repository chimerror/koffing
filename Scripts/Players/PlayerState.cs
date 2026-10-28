using System;
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

	/// <summary>
	/// Updates the state by performing a draw, assigning the <see cref="Tile"/> to <see cref="DrawnTile"/>.
	/// </summary>
	/// <param name="drawnTile">The <see cref="Tile"/> to draw.</param>
	/// <exception cref="ArgumentException">If <paramref name="newlyDrawnTile"/> is <see langword="null"/>.</exception>
	/// <exception cref="InvalidOperationException">
	/// When called while <see cref="DrawnTile"/> is not <see langword="null"/>, indicating a tile has already been
	/// drawn.
	/// </exception>
	public void PerformDraw(Tile newlyDrawnTile)
	{
		if (newlyDrawnTile == null)
		{
			throw new ArgumentException("Attempted to draw null tile!");
		}
		else if (DrawnTile != null)
		{
			throw new InvalidOperationException("Provided new drawn tile while one was already assigned!");
		}

		DrawnTile = newlyDrawnTile;
	}

	/// <summary>
	/// Updates the state by performing a discard, moving the specified <see cref="Tile"/> to <see cref="Discards"/>.
	/// </summary>
	/// <param name="discardIndex">
	/// The index in <see cref="Hand"/> to discard or <c>-1</c> to discard <see cref="DrawnTile"/>.
	/// </param>
	/// <param name="isRiichiDiscard">
	/// If the discard is part of a Riichi claim, and thus should be rotated when put into the discards. If not
	/// specified, defaults to <see langword="false"/>.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If given an index less than <c>-1</c>, an index past the number of tiles in <see cref="Hand"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// When given <c>-1</c> with <see cref="DrawnTile"/> being <see langword="null"/>.
	/// </exception>
	/// <remarks>
	/// This method does not check if the discard is legal as per the swap-calling rules. The validity of the discard
	/// should instead be verified at the time <see cref="Match"/> gets the response from
	/// <see cref="IPlayerLogic.MakeDiscard(Matches.Match, int, IEnumerable{int})"/>.
	/// </remarks>
	public void PerformDiscard(int discardIndex, bool isRiichiDiscard = false)
	{
		if (discardIndex < -1 || discardIndex >= Hand.Count)
		{
			throw new ArgumentException(
				$"Requested invalid discard index {discardIndex} with only {Hand.Count} tiles!",
				nameof(discardIndex));
		}
		else if (discardIndex == -1 && DrawnTile == null)
		{
			throw new InvalidOperationException($"Requested to discard drawn tile with no tile drawn!");
		}

		Tile discardedTile;
		if (discardIndex == -1)
		{
			discardedTile = DrawnTile;
			DrawnTile = null;
		}
		else
		{
			discardedTile = Hand[discardIndex];
			Hand.RemoveAt(discardIndex);

			if (DrawnTile != null)
			{
				Hand.Add(DrawnTile);
				DrawnTile = null;
			}
		}

		if (isRiichiDiscard)
		{
			discardedTile.Rotated = true;
		}
		Discards.Add(discardedTile);
	}
}