using System;

namespace Koffing.Tiles;

/// <summary>
/// Represents a tile.
/// </summary>
public class Tile : IComparable<Tile>, IEquatable<Tile>
{
	/// <summary>
	/// The suit of the tile.
	/// </summary>
	public Suit Suit = Suit.Man;

	/// <summary>
	/// The rank of the tile, which is either a value 1-9 (1-7 for suit Zi) or 0 representing a red five.
	/// </summary>
	public int Rank = 1;

	/// <summary>
	/// If the tile is face-up. In general, this is meant to mean that the tile is face up to all players. UI for a
	/// player's hand is encouraged to just ignore it in favor of showing the player.
	/// </summary>
	public bool FaceUp = true;

	/// <summary>
	/// The "raw" rank of the tile, ignoring red fives. That is, it is the same as <see cref="Rank"/>, unless that is 0,
	/// in which case this returns 5.
	/// </summary>
	/// <value></value>
	public int RawRank
	{
		get => Rank == 0 ? 5 : Rank;
	}

	/// <summary>
	/// Returns if a given suit and rank combination corresponds to a valid tile.
	/// </summary>
	/// <param name="suit">The suit to check.</param>
	/// <param name="rank">The rank to check.</param>
	/// <returns>
	/// If <paramref name="suit"/> is <see cref="Suit.Zi"/> and <paramref name="rank"/> is 1-7, returns
	/// <see langword="true"/>. If <paramref name="suit"/> is any other suit, and <paramref name="rank"/> is 0-9, returns
	/// <see langword="true"/>. Otherwise, returns <see langword="false"/>.
	/// </returns>
	public static bool IsValidTile(Suit suit, int rank)
	{
		return suit != Suit.Zi ? (rank >= 0 && rank <= 9) : (rank >= 1 && rank <= 7);
	}

	/// <summary>
	/// Parameterless constructor. Returns a new tile with suit <see cref="Suit.Man"/> and rank <c>1</c>.
	/// </summary>
	public Tile() : this(Suit.Man, 1)
	{
	}

	/// <summary>
	/// Constructor taking a specified suit and rank.
	/// </summary>
	/// <param name="suit">The suit of the tile.</param>
	/// <param name="rank">The rank of the tile.</param>
	public Tile(Suit suit, int rank = 1)
	{
		Suit = suit;
		Rank = rank;
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Equality is determined by:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		if <paramref name="that"/> is a <see cref="Tile"/>
	/// 	</item>
	/// 	<item>
	/// 		then deferring to <see cref="Equals(Tile)"/>
	/// 	</item>
	/// </list>
	/// </remarks>
	public override bool Equals(object that)
	{
		if ((that == null) || (that is not Tile thatTile))
		{
			return false;
		}

		return Equals(thatTile);
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <remarks>
	/// Equality is determined by:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		if <see cref="Suit"/>s equal
	/// 	</item>
	/// 	<item>
	/// 		if <see cref="Rank"/>s equal
	/// 	</item>
	/// </list>
	/// Note that <see cref="FaceUp"/> is NOT considered.
	/// </remarks>
	/// <param name="that">The <see cref="Tile"/> to compare with the current object.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="that"/> parameter; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	public bool Equals(Tile that)
	{
		if (that == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == that.Suit) && (Rank == that.Rank);
	}

	/// <summary>
	/// Does an equality comparison using <see cref="RawRank"/> instead of <see cref="Rank"/>
	/// </summary>
	/// <remarks>
	/// Equality is determined by:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always false)
	/// 	</item>
	/// 	<item>
	/// 		if <see cref="Suit"/>s equal
	/// 	</item>
	/// 	<item>
	/// 		if <see cref="RawRank"/>s equal
	/// 	</item>
	/// </list>
	/// Note that <see cref="FaceUp"/> is NOT considered.
	/// </remarks>
	/// <param name="that">The tile to compare against.</param>
	/// <returns>
	/// <see langword="true"/> if the current object is equal to the <paramref name="that"/> parameter (using
	/// <see cref="RawRank"/> instead of <see cref="Rank"/>); otherwise, <see langword="false"/>.
	/// </returns>
	public bool RawEquals(Tile that)
	{
		if (that == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == that.Suit) && (RawRank == that.RawRank);
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Calculates the hash value using the integer value of <see cref="Suit"/> raised to one plus the
	/// <see cref="Rank"/>. <see cref="Rank"/> plus one is used to ensure unique hashes.
	/// </remarks>
	public override int GetHashCode()
	{
		var suitInt = (int)Suit;
		var rankInt = Rank + 1; // Increment so the range is 1-10 instead of 0-9, to keep each suit with unique hashes
		return unchecked((int)Math.Pow(suitInt, rankInt));
	}

	/// <summary>
	/// Compares the current instance with another object of the same type and returns an integer that indicates whether
	/// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
	/// </summary>
	/// <remarks>
	/// Comparison is done by comparing:
	/// <list type="number">
	/// 	<item>
	/// 		if <paramref name="that"/> is null (always 1)
	/// 	</item>
	/// 	<item>
	/// 		<see cref="Suit"/>
	/// 	</item>
	/// 	<item>
	/// 		<see cref="Rank"/>, accounting for placing rank 0 (red five) after rank 5.
	/// 	</item>
	/// </list>
	/// </remarks>
	/// <param name="that">A <see cref="Tile"/> to compare with this instance.</param>
	/// <returns>
	/// A value that indicates the relative order of the objects being compared. The return value has these meanings:
	/// <list type="table">
	/// 	<listheader>
	/// 		<term>Value</term>
	/// 		<description>Meaning</description>
	/// 	</listheader>
	/// 	<item>
	/// 		<term>Less than zero</term>
	/// 		<description>This instance precedes <paramref name="that"/> in the sort order.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Zero</term>
	/// 		<description>
	/// 			This instance occurs in the same position in the sort order as <paramref name="that"/>.
	/// 		</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Greater than zero</term>
	/// 		<description>This instance follows <paramref name="that"/> in the sort order.</description>
	/// 	</item>
	/// </list>
	/// </returns>
	public int CompareTo(Tile that)
	{
		if (that == null)
		{
			return 1;
		}

		if (Suit != that.Suit)
		{
			return Suit.CompareTo(that.Suit);
		}
		else if (Rank == that.Rank)
		{
			return 0;
		}
		else if (Rank == 0)
		{
			return that.Rank <= 5 ? 1 : -1;
		}
		else if (that.Rank == 0)
		{
			return Rank <= 5 ? -1 : 1;
		}
		else
		{
			return Rank.CompareTo(that.Rank);
		}
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Prints the MPSZ notation of the tile as generated by <see cref="Extensions.NotationFromTile(Tile)"/>.
	/// </remarks>
	public override string ToString()
	{
		return this.NotationFromTile();
	}
}