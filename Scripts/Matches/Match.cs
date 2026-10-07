using System.Collections.Generic;
using System.Linq;
using Koffing.Players;
using Koffing.Random;
using Koffing.Tiles;
using static Koffing.Helpers;

namespace Koffing.Matches;

public class Match
{
	private Wall _wall;
	private int _currentEastIndex;
	private readonly Dictionary<Wind, Player> _windsToPlayers = [];
	private readonly List<Player> _players;
	private readonly IRandomNumberGenerator _rng;

	public Wind RoundWind { get; private set; } = Wind.East;
	public int NumberOfRounds { get; private set; }
	public PlayerCount PlayerCount { get; private set; }
	public bool HasRedFives { get; private set; }
	public IEnumerable<Player> Players => _players.AsReadOnly();
	public Player CurrentEastPlayer => _players[_currentEastIndex];
	public IDictionary<Wind, Player> WindsToPlayers => _windsToPlayers.AsReadOnly();
	public int CurrentEastIndex => _currentEastIndex;

	// TODO: Think about wrapping this up in a MatchContext class. But then consider if it would make more sense for
	// Wall to know what Match it's being used in instead. This could, for example, be used to change out the RNG if I
	// go ahead with my idea for Cinematic hands.
	public Match(IRandomNumberGenerator rng, PlayerCount playerCount, bool hasRedFives, int numberOfRounds = 2)
	{
		_rng = rng;
		PlayerCount = playerCount;
		HasRedFives = hasRedFives;
		NumberOfRounds = numberOfRounds;
		_wall = new Wall(_rng, PlayerCount, HasRedFives);

		_players = [.. Enumerable.Range(0, (int)PlayerCount)
			.Select(i => new Player()
			{
				Points = PlayerCount == PlayerCount.Four ? 25000 : 30000,
			})
		];
		rng.Shuffle(_players);
		_currentEastIndex = rng.GetIntegerInRange(0, _players.Count - 1);
		UpdateWindsToPlayers();
	}

	// TODO: Public for now, but as we add more of working through match mechanics, I bet it will make sense to make
	// it private, and encapsulate progressing through the game state into something like UpdateGameState
	public void DealHands()
	{
		Dictionary<Wind, List<Tile>> dealtHands = [];
		var setsOfFourToDeal = 3;
		var currentWind = Wind.East;
		do
		{
			for (var currentPlayer = 0; currentPlayer < _players.Count; currentPlayer++)
			{
				List<Tile> dealtTiles = [];
				var tilesToDeal = setsOfFourToDeal > 0 ? 4 : 1;
				for (var currentDealtTile = 0; currentDealtTile < tilesToDeal; currentDealtTile++)
				{
					dealtTiles.Add(_wall.PopNextLiveTile());
				}

				if (!dealtHands.TryGetValue(currentWind, out List<Tile> dealtHand))
				{
					dealtHands[currentWind] = dealtTiles;
				}
				else
				{
					dealtHand.AddRange(dealtTiles);
				}
				currentWind++;
			}

			if (setsOfFourToDeal == 0)
			{
				dealtHands[Wind.East].Add(_wall.PopNextLiveTile());
				break;
			}

			setsOfFourToDeal--;
			currentWind = Wind.East;
		} while (true); // Infinite loop, must break inside

		foreach ((Wind playerWind, List<Tile> playerHand) in dealtHands)
		{
			_windsToPlayers[playerWind].Hand = playerHand;
		}
	}

	private void UpdateWindsToPlayers()
	{
		var currentIndex = _currentEastIndex;
		var currentWind = Wind.East;
		_windsToPlayers.Clear();
		for (var i = 0; i < _players.Count; i++)
		{
			_windsToPlayers.Add(currentWind, _players[currentIndex]);
			currentIndex = GetSafeIndex(++currentIndex, _players.Count);
			currentWind++; // apparently postfix increment doesn't work the same way on enums.
		}
	}
}