namespace cards.Data;

public class EasyLobbyService : ILobbyService
{
    private readonly List<Lobby> _lobbies;

    public EasyLobbyService()
    {
        _lobbies = new List<Lobby>();
    }

    public int CreateLobby(string username, string name, string password)
    {
        _lobbies.Add(new Lobby(username, name, password));

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
}