using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static partial class Extensions
{
	public static List<T> Shuffle<T>(this RandomNumberGenerator rng, IEnumerable<T> originalList)
	{
		var list = new List<T>(originalList);
		var result = new List<T>();
		while (list.Count > 0)
		{
			var indexToRemove = rng.RandiRange(0, list.Count - 1);
			result.Add(list[indexToRemove]);
			list.RemoveAt(indexToRemove);
		}
		return result;
	}

	public static Tile ToTile(this string input)
	{
		return input.ToTiles().Single();
	}

	public static IEnumerable<Tile> ToTiles(this string input)
	{
		foreach (Match match in TileNotationRegex().Matches(input))
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
		yield break;
	}

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
					AppendSuit(currentSuit, builder);
					currentSuit = tile.Suit;
				}
				builder.Append(tile.Rank);
			}
			AppendSuit(currentSuit, builder);
		}

		return builder.ToString();
	}

	private static void AppendSuit(Suit suit, StringBuilder builder)
	{
		switch (suit)
		{
			case Suit.Man:
				builder.Append('m');
				break;

			case Suit.Pin:
				builder.Append('p');
				break;

			case Suit.Sou:
				builder.Append('s');
				break;

			case Suit.Zi:
				builder.Append('z');
				break;
		}
	}

	[GeneratedRegex(@"(?<suitOfTiles>((?<ranks>[0-9]+)(?<suit>[mps])|(?<ranks>[1-7]+)(?<suit>z)))")]
	private static partial Regex TileNotationRegex();

}