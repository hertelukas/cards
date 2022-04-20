namespace cards.Data.Game.Decks;

public class Poker : ICard, IComparable
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

    public int CompareTo(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(Poker))
        {
            return 1;
        }

        var other = (Poker) obj;

        var valCompare = ValueProp.CompareTo(other.ValueProp);
        return valCompare == 0 ? SuitProp.CompareTo(other.SuitProp) : valCompare;
    }

    public static string SpanFromSuit(Suit suit)
    {
        if (suit is Suit.Hearts or Suit.Tiles)
        {
            return $"<span style=\"font-family:'Suits';color:red;\">&#{0xe900 + suit}</span>";
        }

        return $"<span style=\"font-family:Suits;\">&#{0xe900 + suit}</span>";
    }

    public static Queue<Poker> GetDeck()
    {
        var result = new Queue<Poker>();

        foreach (var value in Enum.GetValues<Value>())
        {
            foreach (var suit in Enum.GetValues<Suit>())
            {
                result.Enqueue(new Poker(value, suit));
            }
        }

        return result;
    }

    public enum Suit
    {
        Clovers,
        Pikes,
        Hearts,
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