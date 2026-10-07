using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Koffing.Melds;
using Koffing.Tiles;
using Koffing.Waits;

namespace Koffing.Blocks;

/// <summary>
/// Abstract class representing a grouping of <see cref="Tile"/>s.
/// </summary>
/// <remarks>
/// A block is a very generic grouping of <see cref="Tile"/>s. This should be the root class for all derived blocks as
/// it provides some basic implementations of the major interfaces of <see cref="IEnumerable{T}"/>,
/// <see cref="IBlock"/>, <see cref="IComparable{T}"/>, and <see cref="IEquatable{T}"/>.
/// </remarks>
public abstract class Block : IEnumerable<Tile>, IBlock, IComparable<Block>, IEquatable<Block>
{
	/// <summary>
	/// The tiles that make up the block.
	/// </summary>
	protected readonly List<Tile> _tiles;

	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// <para>
	/// 	In general, this (and the other constructors for derived classes) should not be used in favor of using
	/// 	<see cref="IBlock"/> static functions such as <see cref="IBlock.GetPossible(IEnumerable{Tile})"/> or the
	/// 	<see cref="GetPossibleBlockSets(IEnumerable{Tile})"/> static helper function.
	/// </para>
	/// <para>
	/// 	Those methods will ensure that Blocks created correctly match their intended semantic meaning, such as
	/// 	<see cref="Pung"/>s having all the same tile or <see cref="Chow"/>s having a sequence of tiles in the same
	/// 	suit.
	/// </para>
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the block.</param>
	public Block(IEnumerable<Tile> tiles = null)
	{
		if (tiles != null)
		{
			_tiles = [.. tiles];
		}
		else
		{
			_tiles = [];
		}
	}

	/// <summary>
	/// Indexer.
	/// </summary>
	/// <value>The <see cref="Tile"/> at index <paramref name="index"/> in the Block.</value>
	public Tile this[int index]
	{
		get => _tiles[index];
	}

	/// <summary>
	/// Returns all possible <see cref="BlockSet"/>s that can be created from a collection of <see cref="Tile"/>s.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="BlockSet"/>s.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="BlockSet"/>s that can be created from <paramref name="tiles"/>.
	/// </returns>
	public static IEnumerable<BlockSet> GetPossibleBlockSets(IEnumerable<Tile> tiles)
	{
		var madeBlockSets = new HashSet<BlockSet>();
		foreach (var possibleBlockSet in GetPossibleBlockSetsHelper(tiles))
		{
			if (!madeBlockSets.Add(possibleBlockSet))
			{
				continue;
			}
		}

		var specialHandsBlocks = SevenPairsWait.GetPossible(tiles)
			.Concat(ThirteenOrphansWait.GetPossible(tiles));
		foreach (var madeBlockContext in specialHandsBlocks)
		{
			List<Block> specialHandBlockList = [madeBlockContext.MadeBlock];
			specialHandBlockList.AddRange(madeBlockContext.RemainingTiles.Select(t => new Orphan(t)));
			madeBlockSets.Add(new BlockSet(specialHandBlockList));
		}

		var sortedMadeBlockSets = madeBlockSets.Order();
		foreach (var madeBlockSet in sortedMadeBlockSets)
		{
			yield return madeBlockSet;
		}
	}

	/// <inheritdoc cref="IBlock.GetPossible(IEnumerable{Tile})"/>
	/// <remarks>
	/// As this is an abstract class not meant for concrete use, the implementation of this always throws
	/// <see cref="NotImplementedException"/> to encourage overriding.
	/// </remarks>
	/// <exception cref="NotImplementedException">Thrown to encourage overriding.</exception>
	public static IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		// This is commented out to force implementation, but child classes should override this method with something
		// like the below, replacing `typeof(Block)` with `typeof(ChildBlock)`.
		// return GetPossibleHelper(tiles, typeof(Block)).Distinct();

		throw new NotImplementedException();
	}

	/// <inheritdoc cref="IBlock.GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// <remarks>
	/// As this is an abstract class not meant for concrete use, the implementation of this always throws
	/// <see cref="NotImplementedException"/> to encourage overriding.
	/// </remarks>
	/// <exception cref="NotImplementedException">Thrown to encourage overriding.</exception>
	public static IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		// Throwing because we want to force overriding
		throw new NotImplementedException();
	}

	/// <inheritdoc cref="IBlock.GetHashCodeBasis"/>
	/// <exception cref="NotImplementedException">
	/// Thrown to encourage overriding, as this is an abstract class not meant for concrete use.
	/// </exception>
	public static int GetHashCodeBasis()
	{
		// Throwing because we want to force overriding
		throw new NotImplementedException();
	}

	/// <summary>
	/// Protected static helper function that can be used to simplify overrides of
	/// <see cref="IBlock.GetPossible(IEnumerable{Tile})"/>.
	/// </summary>
	/// <remarks>
	/// Most derived blocks have their logic for the <see cref="IBlock.GetPossible(IEnumerable{Tile})"/> function
	///	implemented as an override of <see cref="IBlock.GetPossibleForTile(Tile, IEnumerable{Tile})"/>, as it is
	/// often easier to write logic around a single given tile. Rather than duplicating the logic to go through all
	/// distinct tiles and calling <see cref="IBlock.GetPossibleForTile(Tile, IEnumerable{Tile})"/> for each one,
	/// this function can be used instead.
	/// </remarks>
	/// <example>
	/// As an example of such an override taken from <see cref="Chow.GetPossible(IEnumerable{Tile})"/>:
	/// <code>
	/// 	public static new IEnumerable&lt;MadeBlockContext&gt; GetPossible(IEnumerable&lt;Tile&gt; tiles)
	///		{
	///			return GetPossibleHelper(tiles, typeof(Chow)).Distinct();
	///		}
	/// </code>
	/// Note that the call to <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> is necessary, as this
	/// function does not do it itself.
	/// </example>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s.</param>
	/// <param name="blockType">The <see cref="Type"/> of block to return</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of type <paramref name="blockType"/>.
	/// </returns>
	// TODO: The comment about having to call Distinct feels like something we can help with a function. Perhaps we can
	// come up with some better names then for this, avoiding calling it or the new function `GetPossibleHelperHelper`.
	protected static IEnumerable<MadeBlockContext> GetPossibleHelper(IEnumerable<Tile> tiles, Type blockType)
	{
		var distinctTiles = tiles.DistinctBy(t => (t.Suit, t.Rank)).ToList();
		foreach (var tile in distinctTiles)
		{
			// TODO: Really don't understand why my ReferenceEquals implementation of this didn't work, but I'm tired
			// of fussing with it.
			var otherTiles = tiles.ToList();
			otherTiles.Remove(tile);

			var getPossibleForTileMethod = blockType.GetMethod(nameof(GetPossibleForTile));
			var possibleBlocks =
				((IEnumerable<MadeBlockContext>)getPossibleForTileMethod.Invoke(null, [tile, otherTiles]))
				.ToList();
			foreach (var madeBlock in possibleBlocks)
			{
				yield return madeBlock;
			}
		}
	}

	private static IEnumerable<BlockSet> GetPossibleBlockSetsHelper(IEnumerable<Tile> tiles)
	{
		var tilesList = tiles.ToList();
		if (tilesList.Count == 0)
		{
			yield return [];
		}
		else if (tilesList.Count == 1)
		{
			yield return new BlockSet([new Orphan(tiles.Single())]);
		}

		bool meldMade = false;
		var firstLevelMelds = Meld.GetFirstLevelMelds(tiles);
		foreach (var firstLevelMeld in firstLevelMelds)
		{
			meldMade = true;
			var madeBlock = firstLevelMeld.MadeBlock;
			var nextLevelBlockSets = GetPossibleBlockSetsHelper(firstLevelMeld.RemainingTiles);
			foreach (var blockSet in nextLevelBlockSets)
			{
				yield return new BlockSet(blockSet.Append(madeBlock));
			}
		}

		if (!meldMade)
		{
			bool waitMade = false;
			var firstLevelWaits = Wait.GetFirstLevelWaits(tiles);
			foreach (var firstLevelWait in firstLevelWaits)
			{
				waitMade = true;
				var madeWait = firstLevelWait.MadeBlock;
				var nextLevelBlockSets = GetPossibleBlockSetsHelper(firstLevelWait.RemainingTiles);
				foreach (var blockSet in nextLevelBlockSets)
				{
					yield return new BlockSet(blockSet.Append(madeWait));
				}
			}

			if (!waitMade)
			{
				yield return new BlockSet(tiles.Select(t => new Orphan(t)));
			}
		}
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		type
	/// 	</item>
	/// 	<item>
	/// 		then deferring to <see cref="Equals(Block)"/>
	/// 	</item>
	/// </list>
	/// </remarks>
	public override bool Equals(object that)
	{
		if ((that == null) ||
			(GetType() != that.GetType()) ||
			(that is not Block thatBlock))
		{
			return false;
		}

		return Equals(thatBlock);
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		type
	/// 	</item>
	/// 	<item>
	/// 		the number of tiles in the block
	/// 	</item>
	/// 	<item>
	/// 		the actual tiles in the block (after sorting)
	/// 	</item>
	/// </list>
	/// </remarks>
	public virtual bool Equals(Block thatBlock)
	{
		if (thatBlock == null)
		{
			return false;
		}

		if ((GetType() != thatBlock.GetType()) ||
			(_tiles.Count != thatBlock._tiles.Count))
		{
			return false;
		}

		var sortedThis = _tiles.Order().ToList();
		var sortedThat = thatBlock._tiles.Order().ToList();
		for (var i = 0; i < sortedThis.Count; i++)
		{
			var currentThis = sortedThis[i];
			var currentThat = sortedThat[i];
			if (!currentThis.Equals(currentThat))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Calculates hash value using the value returned from <see cref="IBlock.GetHashCodeBasis"/> as a basis which is
	/// then exponentiated using an exponent created using that same basis and the hash codes for the tiles of the block.
	/// </remarks>
	public override int GetHashCode()
	{
		unchecked
		{
			// WARNING: I am honestly unsure how good this hashing function is, or if it being bad will actually cause
			// problems. Just keep an eye out.
			var hashCodeBasis = GetActualHashCodeBasis();
			var hashCodeExponent = 1;
			foreach (var tile in _tiles)
			{
				hashCodeExponent = hashCodeExponent * hashCodeBasis + tile.GetHashCode();
			}

			// WARNING: Also worried a bit about the likely conversions to and from float here, but I don't think this
			// will be that used.
			return (int)Math.Pow(hashCodeBasis, hashCodeExponent);
		}
	}

	/// <inheritdoc/>
	public IEnumerator<Tile> GetEnumerator()
	{
		return _tiles.GetEnumerator();
	}

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Comparison is done by comparing:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always 1)
	/// 	</item>
	/// 	<item>
	/// 		type (using <see cref="IBlock.GetHashCodeBasis"/>)
	/// 	</item>
	/// 	<item>
	/// 		the number of tiles in the block
	/// 	</item>
	/// 	<item>
	/// 		the actual tiles in the block (after sorting) using <see cref="Tile.CompareTo(Tile)"/>.
	/// 	</item>
	/// </list>
	/// </remarks>
	public int CompareTo(Block that)
	{
		if (that == null)
		{
			return 1;
		}

		// Because we check type here first, this means that blocks will first be arranged by type and then by their
		// tiles, unlike we'd probably do in a hand. This is OK for our purposes as an engine, but might be something
		// to work around when displaying it to the player. But this way we minimize the checking we have to do as
		// well as avoiding the iterations down below.
		if (GetType() != that.GetType())
		{
			var thisBasis = GetActualHashCodeBasis();
			var thatBasis = that.GetActualHashCodeBasis();
			return thisBasis.CompareTo(thatBasis);
		}

		// The length check here should never be triggered because blocks of the same type should also have the same
		// lengths (though nothing enforces that). I guess this is a fine defensive check, and I could also add variant
		// blocks perhaps in the future that could differ in lengths.
		var thisTiles = this.Order().ToArray();
		var thatTiles = that.Order().ToArray();
		if (thisTiles.Length != thatTiles.Length)
		{
			return thisTiles.Length.CompareTo(thatTiles.Length);
		}

		for (int i = 0; i < thisTiles.Length; i++)
		{
			var thisTile = thisTiles[i];
			var thatTile = thatTiles[i];
			if (thisTile != thatTile)
			{
				return thisTile.CompareTo(thatTile);
			}
		}

		return 0;
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Prints the type (through <see cref="Object.ToString"/>) followed by <c>": "</c> and then the MPSZ notation of
	/// the tiles as generated by <see cref="Extensions.NotationFromTiles(IEnumerable{Tile})"/>.
	/// </remarks>
	// TODOTODO: Should probably have a description of the MPSZ notation formation as a file in the documents.
	// TODO: Might want to add an <examples> showing a few examples to make it clearer. Same for other ToString
	// overrides.
	public override string ToString()
	{
		return base.ToString() + ": " + this.NotationFromTiles();
	}

	private int GetActualHashCodeBasis()
	{
		var blockType = GetType();
		var hashCodeBasisMethod = blockType.GetMethod(nameof(GetHashCodeBasis));
		return (int)hashCodeBasisMethod.Invoke(null, null);
	}
}