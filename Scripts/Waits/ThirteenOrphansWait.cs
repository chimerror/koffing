using System;
using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Melds;
using Koffing.Tiles;

namespace Koffing.Waits;

/// <summary>
/// A wait for the special thirteen orphans hand, made up of single terminals (1 or 9 in a non-Zi suit) and single Zi
/// suit tiles, which may possibly be paired.
/// </summary>
/// <remarks>
/// The intended final meld here is "19m19s19p1234567z" with one of those tiles paired up. Note that
/// <see cref="IBlock.GetPossibleForTile(Tile, IEnumerable{Tile})"/> is NOT overridden as there can be only one possible
/// thirteen orphans wait, and thus no need for it.
/// </remarks>
public class ThirteenOrphansWait : Wait, IBlock
{
	// TODO: With the XML comment I wrote for Melds, I imply that no waits are "complete". For that to actually be true,
	// there should be a way to take special waits such as this and the seven pairs wait to "complete" melds.
	private readonly List<Block> _pairs;

	/// <summary>
	/// The <see cref="Pair"/>s that make up this wait.
	/// </summary>
	/// <value>A read-only <see cref="IEnumerable{T}"/> of the <see cref="Pairs"/> in this wait.</value>
	public IEnumerable<Block> Pairs => _pairs.AsReadOnly();

	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used, and is only defined for testing purposes. Rather, use the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> static helper function in this class to maintain the semantic
	/// meaning of a thirteen orphans wait, as well as properly initializing <see cref="Pairs"/>.
	/// </remarks>
	public ThirteenOrphansWait(IEnumerable<Tile> tiles = null) : base(tiles)
	{
		_pairs = [];
	}

	/// <summary>
	/// Constructor taking an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s representing the tiles in this wait,
	/// and an <see cref="IEnumerable{T}"/> of <see cref="Pair"/>s representing the same tiles, but arranged into pairs.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> static helper function in this class to maintain the semantic
	/// meaning of a thirteen orphans wait.
	/// </remarks>
	/// <param name="tiles">
	/// An <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s representing the tiles in this wait.
	/// </param>
	/// <param name="pairs">
	/// An <see cref="IEnumerable{T}"/> of <see cref="Pair"/>s representing the same tiles, but arranged into pairs.
	/// </param>
	public ThirteenOrphansWait(IEnumerable<Tile> tiles, IEnumerable<Pair> pairs) : base(tiles)
	{
		_pairs = [.. pairs];
	}

	/// <summary>
	/// Get the possible thirteen orphans wait (if any) that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of the possible thirteen orphans wait.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of the thirteen orphans wait that can be
	/// created from <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		var terminalsAndHonors = tiles.Where(t => t.Suit == Suit.Zi || t.Rank == 1 || t.Rank == 9).ToList();
		var simples = tiles.Where(t => t.Suit != Suit.Zi && t.Rank != 1 && t.Rank != 9).ToList();

		if (terminalsAndHonors.Count == 0)
		{
			yield break;
		}

		List<Tile> selectedTiles = [];
		List<Pair> pairs = [];
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

	/// <summary>
	/// Get the hash code basis representing a thirteen orphans wait, which will be exponentiated as a part of
	/// calculating <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for thirteen orphans waits.</returns>
	public static new int GetHashCodeBasis()
	{
		return 31;
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