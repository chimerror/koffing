using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;
using Koffing.Waits;

namespace Koffing.Melds;

/// <summary>
/// An abstract derived <see cref="Block"/> with the semantic meaning of being a "completed" part of a hand.
/// </summary>
/// <remarks>
/// A meld is a <see cref="Block"/> that does not need any additional tiles to be a useful part of a hand, compared to
/// a <see cref="Wait"/>, which is incomplete, and would become a meld given additional tiles.
/// </remarks>
public abstract class Meld : Block
{
	/// <summary>
	/// Returns all possible first-level melds from a collection of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// A "first-level" meld is a single <see cref="Chow"/>, <see cref="Pung"/>, or <see cref="Kong"/> that can be made
	/// from <paramref name="tiles"/>. Note that this does not include <see cref="Pair"/>s. Instead,
	/// <see cref="Wait.GetFirstLevelWaits(IEnumerable{Tile})"/> will generate first-level <see cref="PairWait"/>s,
	/// which can be explicitly cast to <see cref="Pair"/>s.
	/// </remarks>
	/// <param name="tiles">
	/// The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of first-level melds.
	/// </param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of first-level melds that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static IEnumerable<MadeBlockContext> GetFirstLevelMelds(IEnumerable<Tile> tiles)
	{
		return Chow.GetPossible(tiles)
			.Concat(Pung.GetPossible(tiles))
			.Concat(Kong.GetPossible(tiles));
	}

	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// It's not possible to construct a concrete version of this class as it's <see langword="abstract"/>, but it must
	/// be mentioned that even if you could, it is preferred to use <see cref="IBlock"/> static functions such as
	/// <see cref="IBlock.GetPossible(IEnumerable{Tile})"/>, or the
	/// <see cref="GetFirstLevelMelds(IEnumerable{Tile})"/> static helper function in this class to ensure proper
	/// semantic meanings.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the meld.</param>
	public Meld(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}
}