using System;
using System.Collections.Generic;
using System.Linq;
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
	public List<Meld> MadeMelds { get; set; } = [];

	/// <summary>
	/// The claimed pei dora that the player has made.
	/// </summary>
	/// <value>The <see cref="List{T}"/> of pei dora the player has made.</value>
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

	/// <summary>
	/// Updates the state by performing a meld, moving the given <see cref="Meld"/> to <see cref="MadeMelds"/>, and
	/// removing the necessary <see cref="Tile"/>s from the <see cref="Hand"/>.
	/// </summary>
	/// <param name="newlyMadeMeld">
	/// The made <see cref="Meld"/>. It should already be arranged to put down on the table.
	/// </param>
	/// <exception cref="ArgumentException">
	/// If <paramref name="newlyMadeMeld"/> is <see langword="null"/>.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// If it is not possible to make the given meld with the tiles in <see cref="Hand"/> or the
	/// <see cref="DrawnTile"/>.
	/// </exception>
	/// <remarks>
	/// By "already arranged", this means that any claimed <see cref="Tile"/> should be rotated and positioned in the
	/// meld to represent the player claimed from, that promoted kans have the right tiles rotated and stacked, and that
	/// concealed kans have the right tiles face-down. This method will only remove the necessary tiles in the
	/// <see cref="Hand"/>, using the <see cref="Tile.Rotated"/> property for the tiles in the meld to judge which was
	/// the claimed tile.
	/// </remarks>
	public void PerformMeld(Meld newlyMadeMeld)
	{
		if (newlyMadeMeld == null)
		{
			throw new ArgumentException("Null meld passed into PerformMeld!", nameof(newlyMadeMeld));
		}

		if (newlyMadeMeld is Kong newlyMadeKong && newlyMadeKong.KongType != Kong.Type.Called)
		{
			if (newlyMadeKong.KongType == Kong.Type.Concealed)
			{
				var kongTile = newlyMadeKong[0];
				var meldedTiles = (DrawnTile is not null && DrawnTile.RawEquals(kongTile) ? Hand.Append(DrawnTile) : Hand)
					.Where(t => t.RawEquals(kongTile))
					.ToList();
				if (meldedTiles.Count != 4)
				{
					throw new InvalidOperationException($"Attempted to perform concealed kong of \"{kongTile.NotationFromTile()}\" without four matching tiles in hand!");
				}

				if (DrawnTile is not null)
				{
					Hand.Add(DrawnTile);
					DrawnTile = null;
				}

				Hand = [.. Hand.Where(t => !t.RawEquals(kongTile))];
				MadeMelds.Add(newlyMadeKong);
			}
			else
			{
				var kongTile = newlyMadeKong[0];
				var haveTileToPromoteWith =
					(DrawnTile is not null && DrawnTile.RawEquals(kongTile)) ||
					Hand.SingleOrDefault(t => t.RawEquals(kongTile)) != default;
				var havePungToPromote =
					MadeMelds.SingleOrDefault(m => m is Pung && m[0].RawEquals(kongTile)) != default;
				if (!haveTileToPromoteWith)
				{
					throw new InvalidOperationException($"Attempted to perform promoted kong of \"{kongTile.NotationFromTile()}\" without a matching tile in hand or drawn tile!");
				}
				else if (!havePungToPromote)
				{
					throw new InvalidOperationException($"Attempted to perform promoted kong of \"{kongTile.NotationFromTile()}\" without a matching pung in made melds!");
				}

				if (DrawnTile is not null)
				{
					Hand.Add(DrawnTile);
					DrawnTile = null;
				}

				Hand = [.. Hand.Where(t => !t.RawEquals(kongTile))];
				MadeMelds =
				[
					.. MadeMelds.TakeWhile(m => m is not Pung || !m[0].RawEquals(kongTile)),
					newlyMadeKong,
					.. MadeMelds.SkipWhile(m => m is not Pung || !m[0].RawEquals(kongTile)).Skip(1)
				];
			}
		}
		else
		{
			List<Tile> possibleMeldTiles = DrawnTile is not null ? [.. Hand, DrawnTile] : [.. Hand];
			foreach (var meldTile in newlyMadeMeld)
			{
				if (meldTile.Rotated == true)
				{
					continue; // Skip claimed tile
				}

				if (!possibleMeldTiles.Any(t => t.Equals(meldTile)))
				{
					throw new InvalidOperationException($"Attempted to perform called meld \"{newlyMadeMeld}\" but did not find tile \"{meldTile.NotationFromTile()}\" in hand or drawn tile!");
				}

				possibleMeldTiles.Remove(meldTile);
			}

			DrawnTile = null;
			Hand = possibleMeldTiles;
			MadeMelds.Add(newlyMadeMeld);
		}
	}

	/// <summary>
	/// Updates the state by claiming a pei dora from a north tile (4z) from the combination of <see cref="Hand"/> and
	/// <see cref="DrawnTile"/> and adding it to <see cref="PeiDora"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// If there are no north tiles in the combination of <see cref="Hand"/> and <see cref="DrawnTile"/>.
	/// </exception>
	public void PerformPeiDoraClaim()
	{
		if (PeiDora.Count == 4)
		{
			throw new InvalidOperationException("Attempted to claim fifth pei dora!");
		}

		var northTile = new Tile(Suit.Zi, 4);
		var northTiles = (DrawnTile is not null ? Hand.Append(DrawnTile) : Hand)
			.Where(t => t.Equals(northTile))
			.ToList();

		if (northTiles.Count <= 0)
		{
			throw new InvalidOperationException("Attempted to claim pei dora with no north tiles in hand or drawn tile!");
		}

		if (DrawnTile is not null)
		{
			Hand.Add(DrawnTile);
			DrawnTile = null;
		}

		Hand =
		[
			.. Hand.TakeWhile(t => !t.Equals(northTile)),
			.. Hand.SkipWhile(t => !t.Equals(northTile)).Skip(1)
		];
		PeiDora.Add(northTile);
	}
}