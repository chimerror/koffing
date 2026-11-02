using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;

namespace Koffing.Melds;

/// <summary>
/// A meld made up of a quad of four of the same tile, such as 2 Zi, 2 Zi, 2 Zi, 2 Zi ("2222z").
/// </summary>
/// <remarks>
/// Also known as a "kan" in Japanese terms. In this library, that term is saved to mean the call a player makes to form
/// this meld. As always, a red five counts the same as a non-red five.
/// </remarks>
public class Kong : Meld, IBlock, IDowngradable<Pung>
{
	/// <summary>
	/// Represents the type of a <see cref="Kong"/>, that is, how it was created.
	/// </summary>
	/// <remarks>
	/// These values can be used by <see cref="Matches.Match"/> to mark how a kong meld was created, so that it may be
	/// displayed differently based on that, as is standard on the mahjong table. In particular this is either:
	/// <list type="bullet">
	/// 	<item>
	/// 		<see cref="Called"/>, representing a kong that was created by calling the discard from another player
	/// 	</item>
	/// 	<item>
	/// 		<see cref="Promoted"/>, representing a kong that was created by promoting a previously melded
	/// 		<see cref="Pung"/> after drawing the fourth tile
	/// 	</item>
	/// 	<item>
	/// 		<see cref="Concealed"/>, representing a kong that was created by having all four tiles in hand
	/// 	</item>
	/// </list>
	/// </remarks>
	public enum Type
	{
		Called,
		Promoted,
		Concealed,
	}

	/// <summary>
	/// The type of <see cref="Kong"/> the object is, that is, how it was created.
	/// </summary>
	/// <value>A <see cref="Type"/> representing how this <see cref="Kong"/> was created.</value>
	/// <remarks>
	/// This property does participate in equality thanks to <see cref="Equals(Block)"/>, but does NOT participate in
	/// hashing as that has not been overridden, favoring the use of the default <see cref="Block.GetHashCode"/> as
	/// likely to be "good enough".
	/// </remarks>
	public Type KongType { get; set; } = Type.Called;

	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s and the desired
	/// <see cref="Type"/> to set <see cref="KongType"/> to.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a kong.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the kong.</param>
	/// <param name="kongType">The <see cref="Type"/> of the kong.</param>
	public Kong(IEnumerable<Tile> tiles = null, Type kongType = Type.Called) : base(tiles)
	{
		KongType = kongType;
	}

	/// <summary>
	/// Get possible kongs that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of kongs.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kongs that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Kong)).Distinct();
	}

	/// <summary>
	/// Get possible kongs that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created kongs.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of kongs that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		var otherTilesList = otherTiles.ToList();
		var matchingTiles = otherTilesList.Where(t => t.RawEquals(tile)).ToList();
		var nonMatchingTiles = otherTilesList.Where(t => !t.RawEquals(tile)).ToList();

		if (matchingTiles.Count != 3)
		{
			yield break;
		}

		yield return new MadeBlockContext(
			new Kong(matchingTiles.Append(tile)),
			nonMatchingTiles
		);
	}

	/// <summary>
	/// Get the hash code basis representing a kong, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for kongs.</returns>
	public static new int GetHashCodeBasis()
	{
		return 5;
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The proper convention of overriding <see cref="Object.GetHashCode"/> has not been followed, which means that
	/// <see cref="KongType"/>, which this override checks in addition does not participate in hashing. However, as this
	/// lack seems unlikely to ruin the overall hashing of <see cref="Kong"/>s, this violation of convention has been
	/// accepted.
	/// </para>
	/// <para>
	/// Equality is determined by checking:
	/// <list type="number">
	/// 	<item>
	/// 		the result of <see cref="Block.Equals(Block)"/>
	/// 	</item>
	/// 	<item>
	/// 		the <see cref="Kong.Type"/> as reported by <see cref="KongType"/>
	/// 	</item>
	/// </list>
	/// </para>
	/// </remarks>
	/// <param name="thatBlock">The <see cref="Block"/> to compare with the current object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="thatBlock"/> parameter; otherwise,
	/// <see langword="false"/>.
	public override bool Equals(Block thatBlock)
	{
		var baseResult = base.Equals(thatBlock);
		if (!baseResult)
		{
			return false;
		}

		// base should have verified this cast will succeed
		var thatKong = (Kong)thatBlock;
		return KongType == thatKong.KongType;
	}

	IEnumerable<MadeBlockContext> IDowngradable<Pung>.Downgrade()
	{
		var redFive = _tiles.SingleOrDefault(t => t.Rank == 0);
		if (redFive != default)
		{
			var nonRedFives = _tiles.Where(t => t.Rank == 5);
			yield return new MadeBlockContext(
				new Pung(nonRedFives.Take(2).Append(redFive)),
				nonRedFives.Skip(2)
			);
			yield return new MadeBlockContext(
				new Pung(nonRedFives),
				[redFive]
			);
		}
		else
		{
			yield return new MadeBlockContext(
				new Pung(_tiles.Take(3)),
				_tiles.Skip(3)
			);
		}
	}
}