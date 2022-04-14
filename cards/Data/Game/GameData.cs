namespace cards.Data.Game;

public class GameData
{
    public GameData(IEnumerable<string?> cards, IEnumerable<int> otherAmount, string? topCard)
    {
        this.cards = cards;
        this.otherAmount = otherAmount;
        this.topCard = topCard;
    }

    public IEnumerable<string?> cards { get; }
    public IEnumerable<int> otherAmount { get; }
    public string? topCard { get; }
}