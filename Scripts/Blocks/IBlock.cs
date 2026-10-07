using System;
using System.Collections.Generic;
using Koffing.Tests;
using Koffing.Tiles;

namespace Koffing.Blocks;

/// <summary>
/// Interface of static methods used to work with <see cref="Block"/>s.
/// </summary>
/// <remarks>
/// Note that because these are <c>static abstract</c> methods, the <c>new</c> keyword will have to be used on
/// overrides. This also requires calling the correct method through reflection, but for the most part, the code to
/// do that is already part of the <see cref="Block"/> abstract class implementation.
/// </remarks>
public interface IBlock
{
	/// <summary>
	/// Get possible blocks of this type that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	static abstract IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles);

	/// <summary>
	/// Get possible blocks of this type that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created blocks.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	static abstract IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles);

	/// <summary>
	/// Get the hash code basis representing this block type, which will be exponentiated as a part of calculating
	/// <see cref="Object.GetHashCode"/>.
	/// </summary>
	/// <remarks>
	/// The number returned here should be a unique prime number among all derived <see cref="Blocks"/> to ensure proper
	/// hashing. The current set of numbers is verified in
	/// <see cref="BlockTests.GetBlockHashCodeIsCorrect(Block, int)"/>.
	/// </remarks>
	/// <returns>The basis to use when calculating hash codes for this block type.</returns>
	static abstract int GetHashCodeBasis();
}