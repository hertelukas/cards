using System.Collections.ObjectModel;
using cards.Data.Game;

namespace cards.Data;

public class Lobby
{
    private readonly string _password;
    private IGameService _game;
    private readonly List<Player> _players;
    public bool HasStarted { get; private set; }
    public GameEnum SelectedGame { get; private set; }
    public bool HasSelected { get; private set; }

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

    /// <summary>
    /// Set the connection id of a user
    /// </summary>
    /// <param name="username">of the user</param>
    /// <param name="connectionId">of the user, -1 if disconnected</param>
    public void SetConnectionId(string username, string? connectionId)
    {
        _players.First(p => p.Username.Equals(username)).ConnectionId = connectionId;
    }

    public bool DisconnectConnection(string connectionId)
    {
        foreach (var player in _players.Where(player =>
                     player.ConnectionId != null && player.ConnectionId.Equals(connectionId)))
        {
            player.ConnectionId = null;
            return true;
        }

        return false;
    }

    public ReadOnlyCollection<string> GetConnectedUsernames()
    {
        var result = _players.Where(player => player.ConnectionId != null).Select(player => player.Username).ToList();

        return new ReadOnlyCollection<string>(result);
    }

    public void SelectGame(GameEnum game)
    {
        // If game started, nothing will happen
        if (HasStarted) return;

        HasSelected = true;
        SelectedGame = game;
    }

    public void StartGame()
    {
        HasStarted = true;

        switch (SelectedGame)
        {
            case GameEnum.CrazyEights:
                _game = new CrazyEights();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(SelectedGame), SelectedGame, null);
        }

        // Remove all unconnected players
        foreach (var player in _players.Where(player => player.ConnectionId == null))
        {
            _players.Remove(player);
        }

        _game.Initialize(_players.Count);
    }
}