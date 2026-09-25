using System;
using System.Collections.Generic;

public class Wall
{
	private readonly IRandomNumberGenerator _rng;
	private readonly PlayerCount _playerCount;
	private readonly bool _hasRedFives;
	private readonly List<Tile> _tiles = [];

	public IEnumerable<Tile> Tiles => _tiles.AsReadOnly();

	public Wall(IRandomNumberGenerator rng, PlayerCount playerCount, bool hasRedFives)
	{
		_rng = rng;
		_playerCount = playerCount;
		_hasRedFives = hasRedFives;

		foreach (var suit in Enum.GetValues<Suit>())
		{
			for (var rank = 1; rank <= 9; rank++)
			{
				if (suit == Suit.Zi && rank > 7)
				{
					continue;
				}
				else if (_playerCount == PlayerCount.Three && suit == Suit.Man && !(rank == 1 || rank == 9))
				{
					continue;
				}

				for (var instance = 0; instance < 4; instance++)
				{
					var adjustedRank = rank;
					if (_hasRedFives && suit != Suit.Zi && rank == 5 && instance == 0)
					{
						adjustedRank = 0;
					}

					var tile = new Tile(suit, adjustedRank);
					_tiles.Add(tile);
				}
			}
		}

		_tiles = _rng.Shuffle(_tiles);
	}
}