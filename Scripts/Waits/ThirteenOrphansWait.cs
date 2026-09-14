using System.Collections.Generic;
using System.Linq;

public class ThirteenOrphansWait : Wait, IBlock
{
	private readonly List<Block> _pairs;

	public IEnumerable<Block> Pairs => _pairs.AsReadOnly();

	// This constructor does not properly initialize _pairs, it is only here for testing purposes.
	public ThirteenOrphansWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
		_pairs = [];
	}

	public ThirteenOrphansWait(IEnumerable<Tile> tiles, IEnumerable<Block> pairs) : base(tiles)
	{
		_pairs = [.. pairs];
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		var terminalsAndHonors = tiles.Where(t => t.Suit == Suit.Zi || t.Rank == 1 || t.Rank == 9).ToList();
		var simples = tiles.Where(t => t.Suit != Suit.Zi && t.Rank != 1 && t.Rank != 9).ToList();

		List<Tile> selectedTiles = [];
		List<Block> pairs = [];
		var groupedTerminalsAndHonors = terminalsAndHonors.GroupBy(t => (t.Suit, t.Rank)).ToList();
		foreach (var group in groupedTerminalsAndHonors)
		{
			var groupTiles = group.Select(t => t).ToList();

			if (groupTiles.Count == 1)
			{
				selectedTiles.Add(groupTiles[0]);
				continue;
			}

			var groupSelection = groupTiles.Take(2).ToList();
			var notSelected = groupTiles.Skip(2).ToList();
			selectedTiles.AddRange(groupSelection);
			pairs.Add(new Pair(groupSelection));
			simples.AddRange(notSelected);
		}

		// Thankfully, since there are no red terminals/honors, there can only be one possible made block.
		yield return new MadeBlockContext(new ThirteenOrphansWait(selectedTiles, pairs), simples);
	}

	// GetPossibleForTile is NOT overridden because it doesn't really make sense for ThirteenOrphansWait since there is
	// only one possible wait.

	public static new int GetHashCodeBasis()
	{
		return 31;
	}

	// This is exactly the same code from SevenPairsWait.Equals, with variable name changes. Like SevenPairsWait,
	// GetHashCode was not overridden. See the comment on SevenPairsWait.Equals for an explanation of why.
	public override bool Equals(Block thatBlock)
	{
		var baseResult = base.Equals(thatBlock);
		if (!baseResult)
		{
			return false;
		}

		// base should have verified this cast will succeed.
		var thatThirteenOrphans = (ThirteenOrphansWait)thatBlock;

		if (_pairs.Count != thatThirteenOrphans._pairs.Count)
		{
			return false;
		}

		var sortedThisPairs = _pairs.Order().ToList();
		var sortedThatPairs = thatThirteenOrphans._pairs.Order().ToList();
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