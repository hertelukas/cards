namespace cards.Data.Game.Decks;

public class Poker
{
    public class Card : ICard
    {
        public Card(Value value, Suit suit)
        {
            Value = value;
            Suit = suit;
        }

        public Suit Suit { get; }
        public Value Value { get; }

        public string ToString()
        {
            return "<img src=/icons/suits/" + Suit + ".svg width=\"20\"</img> " + Value; 
        }
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