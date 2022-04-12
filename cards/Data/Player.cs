namespace cards.Data;

/// <summary>
/// Represents a CardsUser in a game
/// </summary>
public class Player
{
    private string Username { get; }
    
    public Player(string username)
    {
        Username = username;
    }

}