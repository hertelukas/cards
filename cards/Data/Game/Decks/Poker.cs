namespace cards.Data.Game.Decks;

public class Poker : ICard
{
    public Poker(Value value, Suit suit)
    {
        ValueProp = value;
        SuitProp = suit;
    }

    public Suit SuitProp { get; }
    public Value ValueProp { get; }

    public override string ToString()
    {
        return $"<img src=\"/icons/suits/{SuitProp}.svg\" width=\"20\"</img> {ValueProp}";
    }

    public enum Suit
    {
        Hearts,
        Tiles,
        Clovers,
        Pikes
    }

    public enum Value
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }
}