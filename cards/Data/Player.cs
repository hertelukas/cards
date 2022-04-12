namespace cards.Data;

/// <summary>
/// Represents a CardsUser in a game
/// </summary>
public class Player
{
    public string Username { get; }
    
    public Player(string username)
    {
        Username = username;
    }

}