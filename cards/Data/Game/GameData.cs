namespace cards.Data.Game;

public class GameData
{
    public GameData(IEnumerable<string?> cards, IEnumerable<int> otherAmount, int currentPlayer, string? topCard,
        IEnumerable<string?> features, IEnumerable<bool> featureEnabled)
    {
        Cards = cards;
        OtherAmount = otherAmount;
        TopCard = topCard;
        Features = features;
        FeatureEnabled = featureEnabled;
        CurrentPlayer = currentPlayer;
        OtherUsernames = new List<string>();
    }

    public IEnumerable<string?> Cards { get; }
    public IEnumerable<int> OtherAmount { get; }
    public IEnumerable<string> OtherUsernames { get; set; }
    public int CurrentPlayer { get; }
    public string? TopCard { get; }
    public IEnumerable<string?> Features { get; }
    public IEnumerable<bool> FeatureEnabled { get; }
    public int Id { get; set; }
}