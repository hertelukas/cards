using cards.Data.Enums.Card;

namespace cards.Data.Game.Decks;

public class Poker : ICard, IComparable
{
    public Poker(Value value, Suit suit)
    {
        Value = value;
        Suit = suit;
    }

    public Suit Suit { get; }

    public Value Value { get; }

    public string ToHtmlString()
    {
        return SpanFromSuit(Suit) + $" {Value}";
    }

    public override string ToString()
    {
        return $"{Suit} {Value}";
    }

    public int CompareTo(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(Poker))
        {
            return 1;
        }

        var other = (Poker) obj;

        var valCompare = Value.CompareTo(other.Value);
        return valCompare == 0 ? Suit.CompareTo(other.Suit) : valCompare;
    }

    public static string SpanFromSuit(Suit suit)
    {
        if (suit is Suit.Hearts or Suit.Diamonds)
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
}