using System;
using System.Collections.Generic;
using System.Linq;

public class Penchan : Wait, IBlock
{
	public Penchan(IEnumerable<Tile> tiles = null) : base(tiles)
	{
	}

	public static new IEnumerable<MadeBlockContext> GetPossible(IEnumerable<Tile> tiles)
	{
		return GetPossibleHelper(tiles, typeof(Penchan)).Distinct();
	}

	public static new IEnumerable<MadeBlockContext> GetPossibleForTile(Tile tile, IEnumerable<Tile> otherTiles)
	{
		if (tile.Suit == Suit.Zi || (tile.RawRank > 2 && tile.RawRank < 8))
		{
			yield break;
		}

		var otherTilesList = otherTiles.ToList();

		var partnerRank = tile.RawRank switch
		{
			1 => 2,
			2 => 1,
			8 => 9,
			9 => 8,
			_ => throw new InvalidOperationException($"Tried to make a penchan with invalid rank {tile.RawRank}"),
		};

		var partners = otherTilesList.Where(t => t.Suit == tile.Suit && t.RawRank == partnerRank).ToList();
		var nonPartners = otherTilesList.Where(t => t.Suit != tile.Suit || t.RawRank != partnerRank).ToList();

		if (partners.Count > 0)
		{
			yield return new MadeBlockContext(
				new Penchan(partners.Take(1).Append(tile)),
				nonPartners.Concat(partners.Skip(1))
			);
		}
	}

	public static new int GetHashCodeBasis()
	{
		// TODO: Should we put this in an enum so we can make sure numbers are unique?
		return 23;
	}
}