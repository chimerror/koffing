using System.Collections.Generic;
using System.Linq;
using Koffing.Blocks;
using Koffing.Tiles;
using Koffing.Waits;

namespace Koffing.Melds;

/// <summary>
/// A meld made up of a sequence of three tiles in the same suit, such as 3 Pin, 4 Pin, and 5 Pin ("345p").
/// </summary>
/// <remarks>
/// Also known as a "chii" in Japanese terms. In this library, that term is saved to mean the call a player makes to
/// take a discard and form this meld. Chows cannot be made with tiles of suit Zi, or mixed suits, nor can they "wrap
/// around" such as 9 Sou, 1 Sou, 2 Sou ("912s"). As always, a red five counts the same as a non-red five so "406p"
/// would count as a chow.
/// </remarks>
public class Chow : Meld, IBlock, IDowngradable<Kanchan>, IDowngradable<Penchan>, IDowngradable<Ryanmen>
{
	/// <summary>
	/// Constructor. Optionally takes in an <see cref="IEnumerable{T}"/> of <see cref="Tile"/>s.
	/// </summary>
	/// <remarks>
	/// This constructor should not be used in most cases, instead preferring the
	/// <see cref="GetPossible(IEnumerable{Tile})"/> and <see cref="GetPossibleForTile(Tile, IEnumerable{Tile})"/>
	/// static helper functions in this class to maintain the semantic meaning of a chow.
	/// </remarks>
	/// <param name="tiles">The <see cref="Tile"/>s that make up the chow.</param>
	public Chow(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	/// <summary>
	/// Get possible chows that can be created from a collection of tiles.
	/// </summary>
	/// <param name="tiles">The <see cref="Tile"/>s to use to create <see cref="MadeBlockContext"/>s of chows.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of chows that can be created from
	/// <paramref name="tiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Chow)).Distinct();
	}

	/// <summary>
	/// Get possible chows that can be created using a specified tile and set of other tiles.
	/// </summary>
	/// <param name="tile">The <see cref="Tile"/> that must be included in all created chows.</param>
	/// <param name="otherTiles">An <see cref="IEnumerable{T}"/> of other <see cref="Tile"/>s to use.</param>
	/// <returns>
	/// An <see cref="IEnumerable{T}"/> of <see cref="MadeBlockContext"/>s of chows that can be created from
	/// <paramref name="tile"/> and <paramref name="otherTiles"/>.
	/// </returns>
	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi)
		{
			yield break;
		}

		var firstStartingRank = tile.RawRank - 2;
		if (tile.RawRank <= 2)
		{
			firstStartingRank = 1;
		}
		else if (tile.RawRank > 8)
		{
			firstStartingRank = 7;
		}
		var lastStartingRank = tile.RawRank > 7 ? 7 : tile.RawRank;

		var otherTilesList = otherTiles.ToList();
		for (var startingRank = firstStartingRank; startingRank <= lastStartingRank; startingRank++)
		{
			var lowTiles = GetRankTiles(startingRank, tile, otherTilesList).ToList();
			var middleTiles = GetRankTiles(startingRank + 1, tile, otherTilesList).ToList();
			var highTiles = GetRankTiles(startingRank + 2, tile, otherTilesList).ToList();

			if (lowTiles.Count == 0 || middleTiles.Count == 0 || highTiles.Count == 0)
			{
				continue;
			}

			foreach (var lowTile in lowTiles)
			{
				foreach (var middleTile in middleTiles)
				{
					foreach (var highTile in highTiles)
					{
						yield return new MadeBlockContext(
							new Chow([lowTile, middleTile, highTile]),
							otherTilesList.Where(t => NotChosen(t, lowTile, middleTile, highTile))
						);
					}
				}
			}
		}
	}

	/// <summary>
	/// Get the hash code basis representing a chow, which will be exponentiated as a part of calculating
	/// <see cref="Block.GetHashCode"/>.
	/// </summary>
	/// <returns>The basis to use when calculating hash codes for chows.</returns>
	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put these in an enum so we can make sure numbers are unique?
		return 2;
	}

	private static IEnumerable<Tile> GetRankTiles(int desiredRank, Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.RawRank == desiredRank)
		{
			yield return tile;
		}
		else
		{
			var matchingTiles = otherTiles
				.Where(t => t.Suit == tile.Suit && t.RawRank == desiredRank)
				.GroupBy(t => t.Rank)
				.Select(g => g.First());
			foreach (var matchingTile in matchingTiles)
			{
				yield return matchingTile;
			}
		}
	}

	private static bool NotChosen(Tile candidateTile, Tile lowTile, Tile middleTile, Tile highTile)
	{
		return !(ReferenceEquals(candidateTile, lowTile) ||
			ReferenceEquals(candidateTile, middleTile) ||
			ReferenceEquals(candidateTile, highTile));
	}

	IEnumerable<MadeBlockContext> IDowngradable<Kanchan>.Downgrade()
	{
		var sortedTiles = _tiles.Order().ToList();
		var lowTile = sortedTiles[0];
		var middleTile = sortedTiles[1];
		var highTile = sortedTiles[2];

		yield return new MadeBlockContext(new Kanchan([lowTile, highTile]), [middleTile]);
	}

	IEnumerable<MadeBlockContext> IDowngradable<Penchan>.Downgrade()
	{
		var sortedTiles = _tiles.Order().ToList();
		var lowTile = sortedTiles[0];
		var middleTile = sortedTiles[1];
		var highTile = sortedTiles[2];

		if (lowTile.Rank == 1)
		{
			yield return new MadeBlockContext(new Penchan([lowTile, middleTile]), [highTile]);
		}
		else if (highTile.Rank == 9)
		{
			yield return new MadeBlockContext(new Penchan([middleTile, highTile]), [lowTile]);
		}
	}

	IEnumerable<MadeBlockContext> IDowngradable<Ryanmen>.Downgrade()
	{
		var sortedTiles = _tiles.Order().ToList();
		var lowTile = sortedTiles[0];
		var middleTile = sortedTiles[1];
		var highTile = sortedTiles[2];

		if (lowTile.Rank > 1)
		{
			yield return new MadeBlockContext(new Ryanmen([lowTile, middleTile]), [highTile]);
		}

		if (middleTile.Rank < 8)
		{
			yield return new MadeBlockContext(new Ryanmen([middleTile, highTile]), [lowTile]);
		}
	}
}