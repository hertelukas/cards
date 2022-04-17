using cards.Data.Game.Implementations;
using Xunit;

namespace UnitTests;

public class CrazyEightsUnitTest
{
    [Theory]
    [InlineData(2, 7)]
    [InlineData(3, 5)]
    [InlineData(7, 5)]
    public void AmountOfPlayerCards(int players, int cards)
    {
        var crazyEights = new CrazyEights();

        crazyEights.Initialize(players);

        for (var i = 0; i < players; i++)
        {
            Assert.Equal(cards, crazyEights.GetHand(i).Count);
        }
    }
}