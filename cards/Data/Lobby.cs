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

    public Response JoinLobby(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void SelectGame()
    {
        throw new NotImplementedException();
    }

}