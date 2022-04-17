namespace cards.Data;

public class EasyLobbyService : ILobbyService
{
    private readonly List<Lobby> _lobbies;
    private readonly ILogger<EasyLobbyService> _logger;

    public EasyLobbyService(ILogger<EasyLobbyService> logger)
    {
        _logger = logger;
        _lobbies = new List<Lobby>();
    }

    public int CreateLobby(string username, string password)
    {
        _logger.LogInformation("{Username} created lobby {LobbyId}", username, _lobbies.Count);
        _lobbies.Add(new Lobby(username, password));

        return _lobbies.Count - 1;
    }

    public Lobby GetLobby(int id)
    {
        return _lobbies[id];
    }

    public Response JoinLobby(int id, string password, string username)
    {
        try
        {
            _logger.LogInformation("{Username} is trying to join lobby {LobbyId}", username, id);
            return _lobbies[id].JoinLobby(username, password);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogWarning(e, "Lobby {LobbyId} not found", id);
            return Response.NotFound;
        }
    }

    public bool HasAccess(int id, string username)
    {
        try
        {
            _logger.LogDebug("{Username} is trying to access lobby {LobbyId}", username, id);
            return _lobbies[id].HasAccess(username);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogWarning(e, "Lobby {LobbyId} not found. Not granting access", id);
            return false;
        }
    }

    public void SetConnectionId(int id, string username, string? connectionId)
    {
        try
        {
            _logger.LogDebug("Setting connection id of {Username} to {ConnectionId}", username, connectionId);
            _lobbies[id].SetConnectionId(username, connectionId);
        }
        catch (ArgumentOutOfRangeException e)
        {
            _logger.LogWarning(e, "Lobby {LobbyId} not found. Setting the connection id of {Username} failed", id,
                username);
        }
    }

    public int DisconnectConnection(string connectionId)
    {
        for (var i = 0; i < _lobbies.Count; i++)
        {
            if (_lobbies[i].DisconnectConnection(connectionId))
            {
                _logger.LogInformation("{ConnectionId} disconnected from lobby {LobbyId}", connectionId, i);
                return i;
            }
        }

        _logger.LogWarning("{ConnectionId} not connected to any lobby", connectionId);
        return -1;
    }
}