using System.Collections.Generic;
using cards.Data.Enums.Card;
using cards.Data.Game.Decks;
using cards.Data.Game.Implementations;
using cards.Data.Game.Implementations.CrazyEights;
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

    [Theory]
    [MemberData(nameof(Data))]
    public void TestPlay(Poker topCard, Poker playerCard, bool isPlayable)
    {
        var crazyEights = new CrazyEights
        {
            ShuffleService = list =>
            {
                list[0] = playerCard;
                list[7] = topCard;
            }
        };

        crazyEights.Initialize(1);
        var oldCard = crazyEights.GetGameData()[0].TopCard;

        crazyEights.Play(0, 0);

        var newCard = crazyEights.GetGameData()[0].TopCard;

        Assert.Equal(!isPlayable, newCard != null && newCard.Equals(oldCard));
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[]
            {
                new Poker(Value.Ace, Suit.Hearts),
                new Poker(Value.Seven, Suit.Clovers),
                false
            },
            new object[]
            {
                new Poker(Value.Ace, Suit.Hearts),
                new Poker(Value.Ace, Suit.Clovers),
                true
            },
            new object[]
            {
                new Poker(Value.Ace, Suit.Clovers),
                new Poker(Value.Eight, Suit.Hearts),
                true
            },
            new object[]
            {
                new Poker(Value.Five, Suit.Peaks),
                new Poker(Value.Six, Suit.Clovers),
                false
            }
        };
}