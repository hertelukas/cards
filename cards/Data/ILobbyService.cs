namespace cards.Data;

public interface ILobbyService
{
    /// <summary>
    /// Create a new Lobby
    /// </summary>
    /// <param name="username">The username of the creator of the lobby</param>
    /// <param name="password">A new password for the lobby</param>
    /// <returns>The id of the lobby, -1 if creation failed</returns>
    public int CreateLobby(string username, string password);
    
    
    public Lobby GetLobby(int id);
    public Response JoinLobby(int id, string password, string username);
}