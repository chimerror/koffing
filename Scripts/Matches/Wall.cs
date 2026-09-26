using System;
using System.Collections.Generic;

public class Wall
{
	// The last stack is listed first so that die roll % players is the right index.
	private static readonly int[] _fourPlayerWallStartIndices = [102, 0, 34, 68];
	private static readonly int[] _threePlayerWallStartIndices = [72, 0, 36];

	private readonly List<Tile> _tiles = [];
	private readonly int _liveWallStartIndex;
	private int _nextLiveTileIndex;
	private int _nextReplacementTileIndex;
	private int _lastRevealedDoraIndicatorIndex;
	private int _nextDoraIndicatorIndex;

	public IEnumerable<Tile> Tiles => _tiles.AsReadOnly();

	// These properties are here mostly for testing purposes.
	public int LiveWallStartIndex => _liveWallStartIndex;
	public int LiveWallEndIndex => GetSafeIndex(_liveWallStartIndex - 2);
	public int DeadWallStartIndex => GetSafeIndex(_liveWallStartIndex - 14);
	public int DeadWallEndIndex => GetSafeIndex(_liveWallStartIndex - 1);
	public int NextLiveTileIndex => _nextLiveTileIndex;
	public int NextReplacementTileIndex => _nextReplacementTileIndex;
	public int LastRevealedDoraIndicatorIndex => _lastRevealedDoraIndicatorIndex;
	public int NextDoraIndicatorIndex => _nextDoraIndicatorIndex;

	// TODO: Correctly handle face up and face down. For right now, leaving everything up so WallDisplay doesn't just
	// show face-down tiles.
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

					var tile = new Tile(suit, adjustedRank);
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
		_nextReplacementTileIndex = DeadWallStartIndex;
		_lastRevealedDoraIndicatorIndex = GetSafeIndex(_nextReplacementTileIndex + 4);
		_nextDoraIndicatorIndex = GetSafeIndex(_lastRevealedDoraIndicatorIndex + 2);
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