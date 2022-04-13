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

    public async Task Connect(string username, int lobbyId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());

        _lobbyService.SetConnectionId(lobbyId, username, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId,
            _lobbyService.DisconnectConnection(Context.ConnectionId).ToString());
        await base.OnDisconnectedAsync(exception);
    }
}