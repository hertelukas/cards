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
        return SpanFromSuit(SuitProp) + $" {ValueProp}";
    }

    public override string ToString()
    {
        return $"{SuitProp} {ValueProp}";
    }

    public static string SpanFromSuit(Suit suit)
    {
        if (suit is Suit.Hearts or Suit.Tiles)
        {
            return $"<span style=\"font-family:'Suits';color:red;\">&#{0xe900 + suit}</span>";
        }

        return $"<span style=\"font-family:Suits;\">&#{0xe900 + suit}</span>";
    }

    public enum Suit
    {
        Clovers,
        Hearts,
        Pikes,
        Tiles
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