using cards.Data;
using Microsoft.AspNetCore.SignalR;

namespace cards.Hubs;

public class GameHub : Hub
{
    private readonly ILobbyService _lobbyService;

    public GameHub(ILobbyService lobbyService)
    {
        _lobbyService = lobbyService;
    }

    public async Task ConnectAsync(string username, int lobbyId)
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


    // Send updates
    private async Task SendConnectedUsersUpdateAsync(int lobbyId)
    {
        await Clients.Group(lobbyId.ToString()).SendAsync("ConnectedUsersUpdate",
            _lobbyService.GetLobby(lobbyId).GetConnectedUsernames());
    }
}