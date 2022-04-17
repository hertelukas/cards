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

    /// <summary>
    /// Join a lobby
    /// </summary>
    /// <param name="id">of the lobby</param>
    /// <param name="password">of the lobby</param>
    /// <param name="username">of the user who wants to join</param>
    /// <returns>If the operation was successful. If not, information about the failure.</returns>
    public Response JoinLobby(int id, string password, string username);

    /// <summary>
    /// Check whether a user has access to a lobby
    /// </summary>
    /// <param name="id">of the lobby</param>
    /// <param name="username">of the user</param>
    /// <returns>whether the user has access</returns>
    public bool HasAccess(int id, string username);

    /// <summary>
    /// Set the connectionId of a user
    /// </summary>
    /// <param name="id">of the lobby</param>
    /// <param name="username">of the user</param>
    /// <param name="connectionId">of the connection from the hub. Null if disconnected.</param>
    public void SetConnectionId(int id, string username, string? connectionId);

    /// <summary>
    /// Disconnects a connection from the lobby the connection is connected to
    /// </summary>
    /// <param name="connectionId">of the hub connection</param>
    /// <returns>The id of the lobby the connection was connected to</returns>
    public int DisconnectConnection(string connectionId);
}