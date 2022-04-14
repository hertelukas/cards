namespace cards.Data.Game;

public class GameData
{
    public GameData(IEnumerable<string?> cards, IEnumerable<int> otherAmount, string? topCard)
    {
        Cards = cards;
        OtherAmount = otherAmount;
        TopCard = topCard;
    }

    public IEnumerable<string?> Cards { get; }
    public IEnumerable<int> OtherAmount { get; }
    public string? TopCard { get; }
    public int Id { get; set; }
}