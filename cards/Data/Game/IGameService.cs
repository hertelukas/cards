namespace cards.Data.Game;

public interface IGameService
{
    /// <summary>
    /// The name of the game
    /// </summary>
    /// <returns>Title of the game</returns>
    public string getTitle();
    
    /// <summary>
    /// A description about the game
    /// </summary>
    /// <returns>A detailed description about the rules of the game</returns>
    public string getDescription();
    
    /// <summary>
    /// Get the winner of the current game
    /// </summary>
    /// <returns>The id of the winner, -1 if no winner exists</returns>
    public int getWinner();
}