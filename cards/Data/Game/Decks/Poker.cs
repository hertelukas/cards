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

    public string ToHtmlString()
    {
        return $"<img src=\"/icons/suits/{SuitProp}.svg\" width=\"20\"</img> {ValueProp}";
    }

    public override string ToString()
    {
        return $"{SuitProp} {ValueProp}";
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