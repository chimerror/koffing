using System.Collections.Generic;
using System.Linq;
using Koffing.Players;
using Koffing.Random;
using Koffing.Tiles;
using static Koffing.Helpers;

namespace Koffing.Matches;

/// <summary>
/// Represents a match of mahjong, managing the players playing, the wall, and the current game state.
/// </summary>
public class Match
{
	private Wall _wall;
	private int _currentEastIndex;
	private readonly Dictionary<Wind, Player> _windsToPlayers = [];
	private readonly List<Player> _players;
	private readonly IRandomNumberGenerator _rng;

	/// <summary>
	/// The current round wind.
	/// </summary>
	/// <value>A <see cref="Wind"/> that is the current round wind.</value>
	public Wind RoundWind { get; private set; } = Wind.East;

	/// <summary>
	/// The number of rounds to play in the match.
	/// </summary>
	/// <value>The number of rounds to play in the match.</value>
	public int NumberOfRounds { get; private set; }

	/// <summary>
	/// The number of players in the match.
	/// </summary>
	/// <value>A <see cref="PlayerCount"/> representing the number of players in the match.</value>
	public PlayerCount PlayerCount { get; private set; }

	/// <summary>
	/// Whether or not the match is using red fives in its walls.
	/// </summary>
	/// <value><see langword="true"/> if red fives are being used, <see langword="false"/> otherwise.</value>
	public bool HasRedFives { get; private set; }

	/// <summary>
	/// A read-only collection of the <see cref="Player"/>s in the match.
	/// </summary>
	/// <returns>A read-only collection of the <see cref="Player"/>s playing the match.</returns>
	public IEnumerable<Player> Players => _players.AsReadOnly();

	/// <summary>
	/// The current east <see cref="Player"/>.
	/// </summary>
	public Player CurrentEastPlayer => _players[_currentEastIndex];

	/// <summary>
	/// A read-only dictionary mapping seat <see cref="Wind"/>s to the <see cref="Player"/> currently in the seat.
	/// </summary>
	/// <returns>A read-only dictionary mapping seat <see cref="Wind"/>s to <see cref="Player"/>s.</returns>
	public IDictionary<Wind, Player> WindsToPlayers => _windsToPlayers.AsReadOnly();

	/// <summary>
	/// The index (in <see cref="Players"/>) of the current east <see cref="Player"/>.
	/// </summary>
	public int CurrentEastIndex => _currentEastIndex;

	/// <summary>
	/// Constructor. Creates a new match using the specified <see cref="IRandomNumberGenerator"/> with
	/// <see cref="PlayerCount"/> players, whether to use red fives, and the number of rounds in the match.
	/// </summary>
	/// <remarks>
	/// After this constructor returns, the match will have an initial wall created and broken (as part of the
	/// <see cref="Wall"/> constructor), the players at random indices, and a random player chosen as east. However, no
	/// tiles will be dealt to players yet.
	/// </remarks>
	/// <param name="rng">The <see cref="IRandomNumberGenerator"/> to use for random numbers.</param>
	/// <param name="playerCount">The number of players in the match as a <see cref="PlayerCount"/>.</param>
	/// <param name="hasRedFives">Whether or not red fives should be used.</param>
	/// <param name="numberOfRounds">The number of rounds in the match. If not specified, defaults to 2.</param>
	// TODO: Think about wrapping these match settings up in a MatchContext class. But then consider if it would make
	// more sense for Wall to know what Match it's being used in instead. This could, for example, be used to change out
	// the RNG if I go ahead with my idea for Cinematic hands.
	// TODO: Probably should add some defensive code around the numberOfRounds being a positive number.
	public Match(IRandomNumberGenerator rng, PlayerCount playerCount, bool hasRedFives, int numberOfRounds = 2)
	{
		_rng = rng;
		PlayerCount = playerCount;
		HasRedFives = hasRedFives;
		NumberOfRounds = numberOfRounds;
		_wall = new Wall(_rng, PlayerCount, HasRedFives);

		List<Player> initialPlayers = [.. Enumerable.Range(0, (int)PlayerCount)
			.Select(i => new Player()
			{
				Points = PlayerCount == PlayerCount.Four ? 25000 : 30000,
			})
		];

		// TODO: Right now, our tests don't really check that the players were shuffled, because there isn't an
		// a priori way to know what players are going to end up where. When we add the ability to provide a list of
		// players to a match where we can then check their final seats, make sure that the tests check that shuffling
		// occurs as expected based on the seed.
		_players = rng.Shuffle(initialPlayers);
		_currentEastIndex = rng.GetIntegerInRange(0, _players.Count - 1);
		UpdateWindsToPlayers();
	}

	/// <summary>
	/// Deals the initial starting hands to players.
	/// </summary>
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