namespace cards.Data.Game;

public class GameData
{
    public GameData(IEnumerable<string?> cards, IEnumerable<int> otherAmount, string? topCard, IEnumerable<string?> features)
    {
        Cards = cards;
        OtherAmount = otherAmount;
        TopCard = topCard;
        Features = features;
    }

    public IEnumerable<string?> Cards { get; }
    public IEnumerable<int> OtherAmount { get; }
    public string? TopCard { get; }
    public IEnumerable<string?> Features { get; }
    public int Id { get; set; }
}