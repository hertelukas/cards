namespace cards.Data.Game;

public class GameData
{
    public GameData(IEnumerable<string?> cards, IEnumerable<int> otherAmount, string? topCard,
        IEnumerable<string?> features, IEnumerable<bool> featureEnabled, bool canPlay)
    {
        Cards = cards;
        OtherAmount = otherAmount;
        TopCard = topCard;
        Features = features;
        CanPlay = canPlay;
        FeatureEnabled = featureEnabled;
    }

    public IEnumerable<string?> Cards { get; }
    public IEnumerable<int> OtherAmount { get; }
    public string? TopCard { get; }
    public IEnumerable<string?> Features { get; }
    public IEnumerable<bool> FeatureEnabled { get; }
    public bool CanPlay { get; }
    public int Id { get; set; }
}