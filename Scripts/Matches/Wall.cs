using System;
using System.Collections.Generic;
using Koffing.Random;
using Koffing.Tiles;

using static Koffing.Helpers;

namespace Koffing.Matches;

/// <summary>
/// Represents a wall of <see cref="Tile"/>s.
/// </summary>
/// <remarks>
/// <para>
/// The wall of tiles is the set of shuffled tiles that are used to play a deal of a match. The wall is made up of 136
/// tiles for a four-player or two-player game, and 108 tiles for a three-player game (due to dropping the 2-8 of the
/// Man suit).
/// </para>
/// <para>
/// When created, a wall is already "broken", which means that the point where tiles will be dealt to players and then
/// drawn from has been decided, as the "live" wall. Breaking the wall also marks the "dead" wall, which is made up of
/// 14 tiles in a four-player or two-player game, and 20 tiles in a three-player game. The dead wall is used for
/// drawing replacement tiles for kan calls (and kita calls in three-player games), as well as tracking dora indicators.
/// One dora indicator is revealed when the wall is created. More dora indicators are revealed each time a kan is made.
/// Each indicator indicates a dora tile that receives the dora bonus of one han per tile during scoring.
/// </para>
/// </remarks>
public class Wall
{
	// The last stack is listed first so that die roll % players is the right index.
	private static readonly int[] _fourPlayerWallStartIndices = [102, 0, 34, 68];
	private static readonly int[] _threePlayerWallStartIndices = [72, 0, 36];

	private readonly List<Tile> _tiles = [];
	private readonly int _liveWallStartIndex;
	private readonly int _deadWallCount;
	private int _tilesLeftToPop;
	private int _replacementTilesLeftToPop;
	private int _nextLiveTileIndex;
	private int _nextReplacementTileIndex;
	private int _doraIndicatorsLeftToReveal = 4;
	private int _lastRevealedDoraIndicatorIndex;
	private int _nextDoraIndicatorIndex;

	/// <summary>
	/// A read-only collection of the <see cref="Tile"/>s in the wall.
	/// </summary>
	/// <returns>A read-only collection of the <see cref="Tile"/>s in the wall.</returns>
	public IEnumerable<Tile> Tiles => _tiles.AsReadOnly();

	// TODO: Probably should add properties to get revealed dora and ura dora, a method to reveal a dora, a method to
	// deal a tile, a method to take a replacement tile. These can wait until you get to them in the match code, but I
	// wanted to make a list just in case one gets missed.

	/// <summary>
	/// If a live tile can be taken from the wall.
	/// </summary>
	/// <value><see langword="true"/> if a live tile can be taken, <see langword="false"/> otherwise.</value>
	public bool CanTakeLiveTile => _tilesLeftToPop > 0;

	/// <summary>
	/// If a replacement tile can be taken from the dead wall.
	/// </summary>
	/// <value><see langword="true"/> if a replacement tile can be taken, <see langword="false"/> otherwise.</value>
	public bool CanTakeReplacementTile => _replacementTilesLeftToPop > 0;

	/// <summary>
	/// If a dora indicator can be revealed in the dead wall.
	/// </summary>
	/// <value><see langword="true"/> if a dora indicator can be revealed, <see langword="false"/> otherwise.</value>
	public bool CanRevealDoraIndicator => _doraIndicatorsLeftToReveal > 0;

	// These properties are here mostly for testing purposes, so I am not documenting them.
	// TODO: I forgot about the internal access modifier. We can change it to use that.
	public int LiveWallStartIndex => _liveWallStartIndex;
	public int DeadWallStartIndex => GetSafeIndex(_liveWallStartIndex - 2, _tiles.Count);
	public int DeadWallEndIndex => GetSafeIndex(_liveWallStartIndex - _deadWallCount, _tiles.Count);
	public int NextLiveTileIndex => _nextLiveTileIndex;
	public int NextReplacementTileIndex => _nextReplacementTileIndex;
	public int LastRevealedDoraIndicatorIndex => _lastRevealedDoraIndicatorIndex;
	public int NextDoraIndicatorIndex => _nextDoraIndicatorIndex;

	/// <summary>
	/// Constructor. Takes in an <see cref="IRandomNumberGenerator"/>, a <see cref="PlayerCount"/>, whether or not to
	/// use red fives, and optionally a dice roll to use to break the wall (for testing purposes).
	/// </summary>
	/// <remarks>
	/// Once created, the wall will already be "broken" based on either a dice roll from <paramref name="rng"/>, or a
	/// roll provided by <paramref name="breakDiceRoll"/>, ready to deal tiles. Note that <paramref name="rng"/> will
	/// be used for shuffling the wall regardless of the value of <paramref name="breakDiceRoll"/>.
	/// </remarks>
	/// <param name="rng">The <see cref="IRandomNumberGenerator"/> to use for random numbers.</param>
	/// <param name="playerCount">The number of players in the match as a <see cref="PlayerCount"/>.</param>
	/// <param name="hasRedFives">Whether or not red fives should be used.</param>
	/// <param name="breakDiceRoll">
	/// If specified (set to something besides <c>-1</c>), will be used as the roll to break the wall instead of getting
	/// a dice roll from <paramref name="rng"/>. Parameter only meant for testing purposes.
	/// </param>
	public Wall(IRandomNumberGenerator rng, PlayerCount playerCount, bool hasRedFives, int breakDiceRoll = -1)
	{
		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 1; rank <= 9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					continue;
				}
				else if (playerCount == PlayerCount.Three && suit == Suit.Man && !(rank == 1 || rank == 9))
				{
					continue;
				}

				for (var instance = 0; instance < 4; instance++)
				{
					var adjustedRank = rank;
					if (hasRedFives && suit != Suit.Zi && rank == 5 && instance == 0)
					{
						adjustedRank = 0;
					}

					var tile = new Tile(suit, adjustedRank)
					{
						FaceUp = false
					};
					_tiles.Add(tile);
				}
			}
		}

		_tiles = rng.Shuffle(_tiles);

		var (wallStartIndices, numberOfWallSides) = playerCount == PlayerCount.Three ?
			(_threePlayerWallStartIndices, 3) :
			(_fourPlayerWallStartIndices, 4);
		var dieRoll = breakDiceRoll == -1 ? rng.RollDice() : breakDiceRoll;
		_liveWallStartIndex = wallStartIndices[dieRoll % numberOfWallSides] + (2 * dieRoll);
		_nextLiveTileIndex = _liveWallStartIndex;
		_tilesLeftToPop = playerCount == PlayerCount.Three ? 108 : 136;

		_replacementTilesLeftToPop = playerCount == PlayerCount.Three ? 8 : 4;
		_deadWallCount = playerCount == PlayerCount.Three ? 20 : 14;
		_nextReplacementTileIndex = DeadWallStartIndex;

		// Minus here, because the dead wall runs counter-clockwise
		_lastRevealedDoraIndicatorIndex = GetSafeIndex(
			_nextReplacementTileIndex - _replacementTilesLeftToPop,
			_tiles.Count);

		// By the rules, this shouldn't happen until after the deal, but it won't hurt to do it now.
		_tiles[_lastRevealedDoraIndicatorIndex].FaceUp = true;

		// Minus here, because the dead wall runs counter-clockwise
		_nextDoraIndicatorIndex = GetSafeIndex(_lastRevealedDoraIndicatorIndex - 2, _tiles.Count);
	}

	/// <summary>
	/// If possible, pops the next tile off the live wall, updating indices to point to the tile after it.
	/// </summary>
	/// <returns>
	/// The next live tile in the wall, or <see langword="null"/> if <see cref="CanTakeLiveTile"/> is
	/// <see langword="false"/>.
	/// </returns>
	public Tile PopNextLiveTile()
	{
		if (!CanTakeLiveTile)
		{
			return null;
		}

		var nextTile = _tiles[_nextLiveTileIndex];
		_nextLiveTileIndex = GetSafeIndex(++_nextLiveTileIndex, _tiles.Count);
		_tilesLeftToPop--;
		return nextTile;
	}

	/// <summary>
	/// If possible, pops the next replacement tile off the dead wall, updating indices to point to the tile after it.
	/// </summary>
	/// <returns>
	/// The next replacement tile in the dead wall, or <see langword="null"/> if <see cref="CanTakeReplacementTile"/> is
	/// <see langword="false"/>.
	/// </returns>
	public Tile PopNextReplacementTile()
	{
		if (!CanTakeReplacementTile)
		{
			return null;
		}

		var nextReplacementTile = _tiles[_nextReplacementTileIndex];
		// minus because dead wall runs CCW
		_nextReplacementTileIndex = GetSafeIndex(--_nextReplacementTileIndex, _tiles.Count);
		_tilesLeftToPop--;
		_replacementTilesLeftToPop--;
		return nextReplacementTile;
	}

	/// <summary>
	/// If possible, reveals the next dora indicator in the dead wall, turning it face-up and  updating indices to point
	/// to the tile after it.
	/// </summary>
	/// <returns>
	/// The newly revealed dora indicator in the dead wall, or <see langword="null"/> if
	/// <see cref="CanRevealDoraIndicator"/> is <see langword="false"/>.
	/// </returns>
	public Tile RevealNextDoraIndicator()
	{
		if (!CanRevealDoraIndicator)
		{
			return null;
		}

		var nextIndicator = _tiles[_nextDoraIndicatorIndex];
		nextIndicator.FaceUp = true;
		_lastRevealedDoraIndicatorIndex = _nextDoraIndicatorIndex;
		_nextDoraIndicatorIndex -= 2;
		// minus because dead wall runs CCW
		_nextDoraIndicatorIndex = GetSafeIndex(_nextDoraIndicatorIndex, _tiles.Count);
		_doraIndicatorsLeftToReveal--;
		return nextIndicator;
	}
}