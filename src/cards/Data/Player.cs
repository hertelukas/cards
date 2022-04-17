namespace cards.Data;

/// <summary>
/// Represents a CardsUser in a game
/// </summary>
public class Player
{
    public string Username { get; }
    public string? ConnectionId { get; set; }
    public int Points { get; set; }

    public Player(string username)
    {
        Username = username;
        Points = 0;
    }
}