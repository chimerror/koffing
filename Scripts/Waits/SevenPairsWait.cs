using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait for the special seven pairs hand, made up of one or more pairs, such as "1133m99s55z".
/// </summary>
public class SevenPairsWait : Wait, IBlock
{
	private readonly List<Pair> _pairs;

	/// <summary>
	/// The <see cref="Pair"/>s that make up this wait.
	/// </summary>
	/// <value>A read-only <see cref="IEnumerable{T}"/> of the <see cref="Pairs"/> in this wait.</value>
	public IEnumerable<Pair> Pairs => _pairs.AsReadOnly();

	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used, and is only defined for testing purposes. Rather, use the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a seven pairs wait, as well as
	/// properly initializing <see cref="Pairs"/>.
	/// </remarks>
	public SevenPairsWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
		_pairs = [];
	}

	/// <summary>
	/// Constructor taking an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s representing the tiles in this wait,
	/// and an <see cref="IEnumerable{T}"/> of <see cref="Pair"/>s representing the same tiles, but arranged into pairs.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a seven pairs wait.
	/// </remarks>
	/// <param name="tiles">
	/// An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s representing the tiles in this wait.
	/// </param>
	/// <param name="pairs">
	/// An <see cref="IEnumerable{T}"/> of <see cref="Pair"/>s representing the same tiles, but arranged into pairs.
	/// </param>
	public SevenPairsWait(IEnumerable<Tile> tiles, IEnumerable<Pair> pairs) : base(tiles)
	{
		_pairs = [.. pairs];
	}

	/// <summary>
	/// Get possible seven pair waits that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of seven pair waits.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of seven pair waits that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		List<Pair> nonFivePairs = [];
		List<Tile> remainingNonFives = [];
		List<MadeBlockContext> manFivePairs = [];
		List<MadeBlockContext> souFivePairs = [];
		List<MadeBlockContext> pinFivePairs = [];
		var groupedTiles = tiles.GroupBy(t => (t.Suit, t.RawRank)).ToList();
		foreach (var group in groupedTiles)
		{
			var groupTiles = group.Select(t => t).ToList();
			if (groupTiles.Count == 4)
			{
				nonFivePairs.Add(new Pair([.. groupTiles.Take(2)]));
				nonFivePairs.Add(new Pair([.. groupTiles.Skip(2)]));
			}
			else if (group.Key.Suit != Suit.Zi && group.Key.RawRank == 5)
			{
				if (groupTiles.Count < 2)
				{
					remainingNonFives.Add(groupTiles[0]);
					continue;
				}

				var pairList = group.Key.Suit switch
				{
					Suit.Man => manFivePairs,
					Suit.Sou => souFivePairs,
					Suit.Pin => pinFivePairs,
					_ => throw new InvalidOperationException($"While making seven pairs wait, tried to make a pair with an invalid suit: {group.Key.Suit}"),
				};

				// We are using the non-red five because Pair.GetPossibleForTile operates on the assumption there is no
				// more than one red five, and only generates possibilities considering red 5s when passed a non-red
				// five.
				var nonRedFive = groupTiles.First(t => t.Rank == 5);
				var otherFives = groupTiles.Where(t => !ReferenceEquals(nonRedFive, t));
				var possiblePairs = Pair.GetPossibleForTile(nonRedFive, otherFives);
				pairList.AddRange(possiblePairs);
			}
			else
			{
				if (groupTiles.Count > 1)
				{
					nonFivePairs.Add(new Pair([.. groupTiles.Take(2)]));
					remainingNonFives.AddRange([.. groupTiles.Skip(2)]);
				}
				else
				{
					// Safe to index because if it is not greater than 1 it is 1 because there would be no groupings of
					// 0.
					remainingNonFives.Add(groupTiles[0]);
				}
			}
		}

		List<List<MadeBlockContext>> fivePairsToAdd = [];
		if (manFivePairs.Count > 0)
		{
			fivePairsToAdd.Add(manFivePairs);
		}
		if (souFivePairs.Count > 0)
		{
			fivePairsToAdd.Add(souFivePairs);
		}
		if (pinFivePairs.Count > 0)
		{
			fivePairsToAdd.Add(pinFivePairs);
		}

		if (nonFivePairs.Count == 0 && fivePairsToAdd.Count == 0)
		{
			yield break;
		}
		else if (fivePairsToAdd.Count == 0)
		{
			var pairedTiles = nonFivePairs.SelectMany(p => p);
			yield return new MadeBlockContext(new SevenPairsWait(pairedTiles, nonFivePairs), remainingNonFives);
		}
		else if (fivePairsToAdd.Count == 1)
		{
			foreach (var pairToAdd in fivePairsToAdd[0])
			{
				var allPairs = nonFivePairs.Append((Pair)pairToAdd.MadeBlock);
				var allRemainingTiles = remainingNonFives.Concat(pairToAdd.RemainingTiles);
				var pairedTiles = allPairs.SelectMany(p => p);
				yield return new MadeBlockContext(new SevenPairsWait(pairedTiles, allPairs), allRemainingTiles);
			}
		}
		else if (fivePairsToAdd.Count == 2)
		{
			foreach (var firstPairToAdd in fivePairsToAdd[0])
			{
				foreach (var secondPairToAdd in fivePairsToAdd[1])
				{
					var allPairs = nonFivePairs
						.Append((Pair)firstPairToAdd.MadeBlock)
						.Append((Pair)secondPairToAdd.MadeBlock);
					var allRemainingTiles = remainingNonFives
						.Concat(firstPairToAdd.RemainingTiles)
						.Concat(secondPairToAdd.RemainingTiles);
					var pairedTiles = allPairs.SelectMany(p => p);
					yield return new MadeBlockContext(new SevenPairsWait(pairedTiles, allPairs), allRemainingTiles);
				}
			}
		}
		else
		{
			foreach (var firstPairToAdd in fivePairsToAdd[0])
			{
				foreach (var secondPairToAdd in fivePairsToAdd[1])
				{
					foreach (var thirdPairToAdd in fivePairsToAdd[2])
					{
						var allPairs = nonFivePairs
							.Append((Pair)firstPairToAdd.MadeBlock)
							.Append((Pair)secondPairToAdd.MadeBlock)
							.Append((Pair)thirdPairToAdd.MadeBlock);
						var allRemainingTiles = remainingNonFives
							.Concat(firstPairToAdd.RemainingTiles)
							.Concat(secondPairToAdd.RemainingTiles)
							.Concat(thirdPairToAdd.RemainingTiles);
						var pairedTiles = allPairs.SelectMany(p => p);
						yield return new MadeBlockContext(new SevenPairsWait(pairedTiles, allPairs), allRemainingTiles);
					}
				}
			}
		}
	}

	/// <summary>
	/// Get possible seven pair waits that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created seven pair waits.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of seven pair waits that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		return GetPossible(otherTiles.Append(tile))
		.Where(mbc => mbc.MadeBlock.Any(t => ReferenceEquals(tile, t)));
	}

	/// <summary>
	/// Get the hash code basis representing a seven pair wait, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for seven pair waits.</returns>
	public static new int GetHashCodeBasis()
	{
		return 29;
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This method should not generally be used, and is only present for testing purposes. As such, the proper
	/// convention of overriding <see cref="Object.GetHashCode"/> has not been followed.
	/// </para>
	/// <para>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		the result of <see cref="Block.Equals(Block)"/>
	/// 	</item>
	/// 	<item>
	/// 		the count of pairs in <see cref="Pairs"/>
	/// 	</item>
	/// 	<item>
	/// 		the actual pairs in <see cref="Pairs"/> (after sorting)
	/// 	</item>
	/// </list>
	/// </para>
	/// </remarks>
	/// <param name="thatBlock">The <see cref="Block"/> to compare with the current object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="thatBlock"/> parameter; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	// Did not override GetHashCode even though we probably should in theory. The goal of the override was to make sure
	// the blocking into pairs works properly in tests. But in regard to possible differences between values, as long
	// as the blocking algorithm is not wrong, the list of pairs should not reveal any differences that we'd not notice
	// in the list of tiles.
	public override bool Equals(Block thatBlock)
	{
		var baseResult = base.Equals(thatBlock);
		if (!baseResult)
		{
			return false;
		}

		// base should have verified this cast will succeed.
		var thatSevenPairs = (SevenPairsWait)thatBlock;

		if (_pairs.Count != thatSevenPairs._pairs.Count)
		{
			return false;
		}

		var sortedThisPairs = _pairs.Order().ToList();
		var sortedThatPairs = thatSevenPairs._pairs.Order().ToList();
		for (var i = 0; i < sortedThatPairs.Count; i++)
		{
			var currentThis = sortedThisPairs[i];
			var currentThat = sortedThatPairs[i];
			if (!currentThis.Equals(currentThat))
			{
				return false;
			}
		}

		return true;
	}
}