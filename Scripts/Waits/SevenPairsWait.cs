using System;
using System.Collections.Generic;
using System.Linq;

public class SevenPairsWait : Wait, IBlock
{
	private readonly List<Block> _pairs;

	public IEnumerable<Block> Pairs => _pairs.AsReadOnly();

	// This constructor does not properly initialize _pairs, it is only here for testing purposes.
	public SevenPairsWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
		_pairs = [];
	}

	public SevenPairsWait(IEnumerable<Tile> tiles, IEnumerable<Block> pairs) : base(tiles)
	{
		_pairs = [.. pairs];
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		List<Block> nonFivePairs = [];
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

		if (fivePairsToAdd.Count == 0)
		{
			var pairedTiles = nonFivePairs.SelectMany(p => p);
			yield return new MadeBlockContext(new SevenPairsWait(pairedTiles, nonFivePairs), remainingNonFives);
		}
		else if (fivePairsToAdd.Count == 1)
		{
			foreach (var pairToAdd in fivePairsToAdd[0])
			{
				var allPairs = nonFivePairs.Append(pairToAdd.MadeBlock);
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
						.Append(firstPairToAdd.MadeBlock)
						.Append(secondPairToAdd.MadeBlock);
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
							.Append(firstPairToAdd.MadeBlock)
							.Append(secondPairToAdd.MadeBlock)
							.Append(thirdPairToAdd.MadeBlock);
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

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		return GetPossible(otherTiles.Append(tile))
		.Where(mbc => mbc.MadeBlock.Any(t => ReferenceEquals(tile, t)));
	}

	public static new int GetHashCodeBasis()
	{
		return 29;
	}

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