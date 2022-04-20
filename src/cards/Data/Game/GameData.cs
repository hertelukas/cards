namespace cards.Data.Game;

/// <summary>
/// This class represents the data send to a specific player
/// </summary>
public class GameData
{
    /// <summary>
    /// Constructor for the representation of a game based on a single players view
    /// </summary>
    /// <param name="cards">The current hand of the player</param>
    /// <param name="otherAmount">How many cards the other players have, starting with the player who plays after the receiver of this data</param>
    /// <param name="currentPlayer">The index of the current player, starting with 0 as the receiving player</param>
    /// <param name="topCard">A string that represents the top card and can hold additional information about the state of the game</param>
    /// <param name="features">A list of features, (moves), that a player could execute</param>
    /// <param name="featureEnabled">A list of booleans, whether the feature with the same index can be executed</param>
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
    public string? TopCard { get; set; }
    public IEnumerable<string?> Features { get; }
    public IEnumerable<bool> FeatureEnabled { get; }
    public int Id { get; set; }
}