using System;

public class Tile : IComparable<Tile>, IEquatable<Tile>
{
	public Suit Suit = Suit.Man;
	public int Rank = 1;
	public bool FaceUp = true;

	public int RawRank
	{
		get => Rank == 0 ? 5 : Rank;
	}

	public Tile() : this(Suit.Man, 1)
	{
	}

	public Tile(Suit suit, int rank = 1)
	{
		Suit = suit;
		Rank = rank;
	}

	public override bool Equals(object that)
	{
		if ((that == null) || (that is not Tile thatTile))
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return Equals(thatTile);
	}

	public bool Equals(Tile that)
	{
		if (that == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == that.Suit) && (Rank == that.Rank);
	}

	public bool RawEquals(Tile that)
	{
		if (that == null)
		{
			return false;
		}

		// Face-up doesn't count for this equals.
		return (Suit == that.Suit) && (RawRank == that.RawRank);
	}

	public override int GetHashCode()
	{
		var suitInt = (int)Suit;
		var rankInt = Rank + 1; // Increment so the range is 1-10 instead of 0-9, to keep each suit with unique hashes
		return unchecked((int)Math.Pow(suitInt, rankInt));
	}

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

	public override string ToString()
	{
		return this.NotationFromTile();
	}
}