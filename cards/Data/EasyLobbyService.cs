namespace cards.Data;

public class EasyLobbyService : ILobbyService
{
    private readonly List<Lobby> _lobbies;

    public EasyLobbyService()
    {
        _lobbies = new List<Lobby>();
    }

    public int CreateLobby(string username, string password)
    {
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
            return _lobbies[id].JoinLobby(username, password);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Response.NotFound;
        }
    }

    public bool HasAccess(int id, string username)
    {
        try
        {
            return _lobbies[id].HasAccess(username);
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    public void SetConnectionId(int id, string username, string? connectionId)
    {
        try
        {
            _lobbies[id].SetConnectionId(username, connectionId);
        }
        catch (ArgumentOutOfRangeException)
        {
            
        }
    }

    public int DisconnectConnection(string connectionId)
    {
        for (var i = 0; i < _lobbies.Count; i++)
        {
            if (_lobbies[i].DisconnectConnection(connectionId))
            {
                return i;
            }
        }

        return -1;
    }
}