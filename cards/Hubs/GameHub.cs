using cards.Data;
using cards.Data.Game;
using Microsoft.AspNetCore.SignalR;

namespace cards.Hubs;

public class GameHub : Hub
{
    private readonly ILobbyService _lobbyService;

    public GameHub(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    #region Connections

    public async Task Connect(string username, int lobbyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());

        _lobbyService.SetConnectionId(lobbyId, username, Context.ConnectionId);

        await SendConnectedUsersUpdateAsync(lobbyId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobbyId = _lobbyService.DisconnectConnection(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
        await SendConnectedUsersUpdateAsync(lobbyId);
        await base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region Receive

    public async Task ReceiveSelectGame(int lobbyId, GameEnum game)
    {
        _lobbyService.GetLobby(lobbyId).SelectGame(game);
        await SendSelectedGameAsync(lobbyId, game);
    }

    public async Task ReceiveStartGame(int lobbyId)
    {
        _lobbyService.GetLobby(lobbyId).StartGame();
        await SendGameUpdateAsync(lobbyId);
    }

    #endregion

    #region Send

    private async Task SendConnectedUsersUpdateAsync(int lobbyId)
    {
        await Clients.Group(lobbyId.ToString()).SendAsync("ConnectedUsersUpdate",
            _lobbyService.GetLobby(lobbyId).GetConnectedUsernames());
    }

    private async Task SendSelectedGameAsync(int lobbyId, GameEnum game)
    {
        await Clients.Group(lobbyId.ToString()).SendAsync("SelectedGameUpdate", game);
    }

    private async Task SendGameUpdateAsync(int lobbyId)
    {
        var lobby = _lobbyService.GetLobby(lobbyId);

        var gameData = lobby.GetGameData();

        for (var i = 0; i < gameData.Count; i++)
        {
            var connectionId = lobby.GetConnectionId(i);
            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("GameUpdate", gameData[i]);
            }
        }
    }

    #endregion
}