using System;
using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class TileTests
{
    [TestCase]
    public void RawRanksAreCorrect()
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            for (var rank = 0; rank <= 9; rank++)
            {
                if (suit == Suit.Zi && (rank == 0 || rank > 7))
                {
                    continue;
                }

                var tile = new Tile(suit, rank);
                var expectedRawRank = rank == 0 ? 5 : rank;
                AssertThat(tile.RawRank).IsEqual(expectedRawRank);
            }
        }
    }
}