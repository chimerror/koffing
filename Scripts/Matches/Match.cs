using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using static Helpers;

public class Match
{
	private Wall _wall;
	private int _currentEastIndex;
	private Dictionary<Wind, Player> _windsToPlayers;
	private readonly List<Player> _players;
	private readonly IRandomNumberGenerator _rng;

	public Wind RoundWind { get; private set; } = Wind.East;
	public int NumberOfRounds { get; private set; }
	public PlayerCount PlayerCount { get; private set; }
	public bool HasRedFives { get; private set; }
	public IEnumerable<Player> Players => _players.AsReadOnly();
	public Player CurrentEastPlayer => _players[_currentEastIndex];
	public ReadOnlyDictionary<Wind, Player> WindsToPlayers => _windsToPlayers.AsReadOnly();
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
				// TODOTODO: Should 2-player use a different number of points?
				// TODOTODO: On Yakuman GB, 2-player uses 30000 points, so that's what I'll do.
				Points = PlayerCount == PlayerCount.Three ? 30000 : 25000
			})
		];
		rng.Shuffle(_players);
		_currentEastIndex = rng.GetIntegerInRange(0, _players.Count - 1);

		var currentIndex = _currentEastIndex;
		var currentWind = Wind.East;
		_windsToPlayers = [];
		for (var i = 0; i < _players.Count; i++)
		{
			_windsToPlayers.Add(currentWind, _players[currentIndex]);
			currentIndex = GetSafeIndex(++currentIndex, _players.Count);
			currentWind++; // apparently postfix increment doesn't work the same way on enums.
		}
	}
}