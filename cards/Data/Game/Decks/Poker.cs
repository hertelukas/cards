namespace cards.Data.Game.Decks;

public class Poker
{
    public record Card(Suit Suit, Value Value);
    public enum Suit
    {
        Hearts,
        Tiles,
        Clovers,
        Pikes
    }

    public enum Value
    {
        One,
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