using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Koffing.Random;
using Koffing.Tiles;
using RE = System.Text.RegularExpressions;

namespace Koffing;

/// <summary>
/// Useful extension methods for various purposes.
/// </summary>
public static partial class Extensions
{
	/// <summary>
	/// Shuffles an <see cref="IEnumerable{T}" /> using an <see cref="IRandomNumberGenerator"/>, copying the elements
	/// to a new list.
	/// </summary>
	/// <param name="rng">The <see cref="IRandomNumberGenerator"/> to use for shuffling.</param>
	/// <param name="originalList">An <see cref="IEnumerable{T}"/> to shuffle. It will not be modified.</param>
	/// <typeparam name="T">The type of elements in <paramref name="originalList"/>.</typeparam>
	/// <returns>A new list with the elements of <paramref name="originalList"/> shuffled.</returns>
	public static List<T> Shuffle<T>(this IRandomNumberGenerator rng, IEnumerable<T> originalList)
	{
		var list = new List<T>(originalList);
		var result = new List<T>();
		while (list.Count > 0)
		{
			var indexToRemove = rng.GetIntegerInRange(0, list.Count - 1);
			result.Add(list[indexToRemove]);
			list.RemoveAt(indexToRemove);
		}
		return result;
	}

	/// <summary>
	/// Uses an <see cref="IRandomNumberGenerator"/> to simulate rolling dice.
	/// </summary>
	/// <param name="rng">The <see cref="IRandomNumberGenerator"/> to use for dice rolling.</param>
	/// <param name="numberOfDice">The number of dice to roll. Assumes 2 if not specified.</param>
	/// <param name="sidesOfDie">The number of sides on each die. Assumes 6 if not specified.</param>
	/// <returns>
	/// A random total from rolling <paramref name="numberOfDice"/> dice with <paramref name="sidesOfDie"/> sides.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// If given a non-positive <paramref name="numberOfDice"/> or <paramref name="sidesOfDie"/>.
	/// </exception>
	public static int RollDice(this IRandomNumberGenerator rng, int numberOfDice = 2, int sidesOfDie = 6)
	{
		if (numberOfDice <= 0)
		{
			throw new ArgumentException($"Non-positive number of dice requested: {numberOfDice}", nameof(numberOfDice));
		}
		if (sidesOfDie <= 0)
		{
			throw new ArgumentException(
				$"Non-positive number of die sides requested: {sidesOfDie}",
				nameof(sidesOfDie));
		}

		var totalRoll = 0;
		for (int currentRoll = 0; currentRoll < numberOfDice; currentRoll++)
		{
			totalRoll += rng.GetIntegerInRange(1, sidesOfDie);
		}
		return totalRoll;
	}

	/// <summary>
	/// Converts a string in MPSZ notation to a <see cref="Tile"/>.
	/// </summary>
	/// <param name="input">A string in MPSZ notation describing a single tile.</param>
	/// <returns>The created <see cref="Tile"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if <paramref name="input"/> would produce no tile (likely due to invalid MPSZ syntax) or more than one
	/// tile.
	/// </exception>
	public static Tile ToTile(this string input)
	{
		var tiles = input.ToTiles().ToList();
		return tiles.Single();
	}

	/// <summary>
	/// Converts a string in MPSZ notation to one or more tiles.
	/// </summary>
	/// <param name="input">A string in MPSZ notation describing one or more tiles.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s created from <paramref name="input"/>.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if <paramref name="input"/> was not in valid MPSZ notation or was the empty string.
	/// </exception>
	// TODO: Consider if we have need to call this with the empty string, and should allow it to return an empty
	// collection.
	public static IEnumerable<Tile> ToTiles(this string input)
	{
		if (!FullTileNotationRegex().IsMatch(input))
		{
			throw new InvalidOperationException($"Input was not in valid MPSZ notation: {input}");
		}

		foreach (RE.Match match in TileNotationGroupRegex().Matches(input))
		{
			var stringSuit = match.Groups["suit"].Value;
			var suit = Suit.Man;
			suit = stringSuit switch
			{
				"m" => Suit.Man,
				"p" => Suit.Pin,
				"s" => Suit.Sou,
				"z" => Suit.Zi,
				_ => throw new InvalidOperationException($"Unknown suit encountered: {stringSuit}"),
			};
			var stringRanks = match.Groups["ranks"].Value;
			foreach (var charRank in stringRanks)
			{
				var rank = int.Parse(charRank.ToString());
				yield return new Tile(suit, rank);
			}
		}
	}

	/// <summary>
	/// Generates the MPSZ notation for a single tile.
	/// </summary>
	/// <param name="input">The <see cref="Tile"/> to convert to notation.</param>
	/// <returns>The MPSZ notation describing <paramref name="input"/>.</returns>
	public static string NotationFromTile(this Tile input)
	{
		return new Tile[] { input }.NotationFromTiles();
	}

	/// <summary>
	/// Generates the MPSZ notation for a collection of tiles.
	/// </summary>
	/// <remarks>
	/// If given an empty collection, the empty string will be returned.
	/// </remarks>
	/// <param name="input">An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s to convert to notation.</param>
	/// <returns>The MPSZ notation describing <paramref name="input"/></returns>
	public static string NotationFromTiles(this IEnumerable<Tile> input)
	{
		var builder = new StringBuilder();
		var tiles = input.ToList();
		if (tiles.Count > 0)
		{
			var currentSuit = tiles[0].Suit;
			foreach (var tile in tiles)
			{
				if (tile.Suit != currentSuit)
				{
					builder.Append(currentSuit.GetSuitString());
					currentSuit = tile.Suit;
				}
				builder.Append(tile.Rank);
			}
			builder.Append(currentSuit.GetSuitString());
		}

		return builder.ToString();
	}

	/// <summary>
	/// Returns the MPSZ notation string for the specified suit.
	/// </summary>
	/// <param name="suit">The <see cref="Suit"/> to covert to notation.</param>
	/// <returns>
	/// <list type="table">
	/// 	<listheader>
	/// 		<term>Suit</term>
	/// 		<description>Result</description>
	/// 	</listheader>
	/// 	<item>
	/// 		<term>Man</term>
	/// 		<description><c>"m"</c></description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Pin</term>
	/// 		<description><c>"p"</c></description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Sou</term>
	/// 		<description><c>"s"</c></description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Zi</term>
	/// 		<description><c>"z"</c></description>
	/// 	</item>
	/// </list>
	/// </returns>
	/// <exception cref="InvalidOperationException">If given an invalid <see cref="Suit"/>.</exception>
	public static string GetSuitString(this Suit suit)
	{
		return suit switch
		{
			Suit.Man => "m",
			Suit.Sou => "s",
			Suit.Pin => "p",
			Suit.Zi => "z",
			_ => throw new InvalidOperationException($"Invalid suit passed into GetSuitString: {suit}"),
		};
	}

	[RE.GeneratedRegex(@"(?<suitOfTiles>((?<ranks>[0-9]+)(?<suit>[mps])|(?<ranks>[1-7]+)(?<suit>z)))")]
	private static partial RE.Regex TileNotationGroupRegex();

	[RE.GeneratedRegex(@"^(?<suitOfTiles>((?<ranks>[0-9]+)(?<suit>[mps])|(?<ranks>[1-7]+)(?<suit>z)))+$")]
	private static partial RE.Regex FullTileNotationRegex();
}