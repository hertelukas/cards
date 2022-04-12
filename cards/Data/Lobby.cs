using cards.Data.Game;

namespace cards.Data;

public class Lobby
{
    private readonly string _name;
    private readonly string _password;
    private IGameService _game;
    private List<Player> _players;

    public Lobby(string username, string name, string password)
    {
        _players = new List<Player> {new(username)};
        _name = name;
        _password = password;
    }

    public Response JoinLobby(string username, string password)
    {
        if (!_password.Equals(password)) return Response.InvalidPassword;

        _players.Add(new Player(username));
        return Response.Success;
    }

    public void SelectGame()
    {
        throw new NotImplementedException();
    }
}