using cards.Data.Game;

namespace cards.Data;

public class Lobby
{
    private readonly string _password;
    private IGameService _game;
    private List<Player> _players;

    public Lobby(string username, string password)
    {
        _players = new List<Player> {new(username)};
        _password = password;
    }

    /// <summary>
    /// Let a user join a lobby. If the user is already in the lobby, SUCCESS gets returned and nothing happens.
    /// </summary>
    /// <param name="username">of the user that wants to join the lobby</param>
    /// <param name="password">of this lobby</param>
    /// <returns>If the action was successful</returns>
    public Response JoinLobby(string username, string password)
    {
        if (!_password.Equals(password)) return Response.InvalidPassword;

        // Check if player is already in the list
        if (!_players.Exists(p => p.Username.Equals(username)))
        {
            _players.Add(new Player(username));
        }

        return Response.Success;
    }

    /// <summary>
    /// Check if a user is in this lobby 
    /// </summary>
    /// <param name="username">of the user that should get checked</param>
    /// <returns>Whether the given user is in this lobby</returns>
    public bool HasAccess(string username)
    {
        return _players.Exists(p => p.Username.Equals(username));
    }

    public void SelectGame()
    {
        throw new NotImplementedException();
    }
}