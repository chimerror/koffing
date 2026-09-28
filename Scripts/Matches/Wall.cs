using System;
using System.Collections.Generic;

public class Wall
{
	// The last stack is listed first so that die roll % players is the right index.
	private static readonly int[] _fourPlayerWallStartIndices = [102, 0, 34, 68];
	private static readonly int[] _threePlayerWallStartIndices = [72, 0, 36];

	private readonly List<Tile> _tiles = [];
	private readonly int _liveWallStartIndex;
	private readonly int _deadWallCount;
	private int _tilesLeftToPop;
	private int _nextLiveTileIndex;
	private int _nextReplacementTileIndex;
	private int _lastRevealedDoraIndicatorIndex;
	private int _nextDoraIndicatorIndex;

	public IEnumerable<Tile> Tiles => _tiles.AsReadOnly();

	// TODO: Probably should add properties to get revealed dora and ura dora, a method to reveal a dora, a method to
	// deal a tile, a method to take a replacement tile. These can wait until you get to them in the match code, but I
	// wanted to make a list just in case one gets missed.

	// These properties are here mostly for testing purposes.
	public int LiveWallStartIndex => _liveWallStartIndex;
	public int DeadWallStartIndex => GetSafeIndex(_liveWallStartIndex - 2);
	public int DeadWallEndIndex => GetSafeIndex(_liveWallStartIndex - _deadWallCount);
	public int NextLiveTileIndex => _nextLiveTileIndex;
	public bool CanTakeLiveTile => _tilesLeftToPop > 0;
	public int NextReplacementTileIndex => _nextReplacementTileIndex;
	public int LastRevealedDoraIndicatorIndex => _lastRevealedDoraIndicatorIndex;
	public int NextDoraIndicatorIndex => _nextDoraIndicatorIndex;

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

		var numberOfReplacementTiles = playerCount == PlayerCount.Three ? 8 : 4;
		_deadWallCount = playerCount == PlayerCount.Three ? 20 : 14;
		_nextReplacementTileIndex = DeadWallStartIndex;

		// Minus here, because the dead wall runs counter-clockwise
		_lastRevealedDoraIndicatorIndex = GetSafeIndex(_nextReplacementTileIndex - numberOfReplacementTiles);

		// By the rules, this shouldn't happen until after the deal, but it won't hurt to do it now.
		_tiles[_lastRevealedDoraIndicatorIndex].FaceUp = true;

		// Minus here, because the dead wall runs counter-clockwise
		_nextDoraIndicatorIndex = GetSafeIndex(_lastRevealedDoraIndicatorIndex - 2);
	}

	public Tile PopNextLiveTile()
	{
		if (!CanTakeLiveTile)
		{
			return null;
		}

		var nextTile = _tiles[_nextLiveTileIndex];
		_nextLiveTileIndex = GetSafeIndex(++_nextLiveTileIndex);
		_tilesLeftToPop--;
		return nextTile;
	}

	private int GetSafeIndex(int index)
	{
		if (index < 0)
		{
			return index + _tiles.Count;
		}
		else if (index >= _tiles.Count)
		{
			return index - _tiles.Count;
		}
		else
		{
			return index;
		}
	}
}