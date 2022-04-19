using cards.Data;
using cards.Data.Game;
using Microsoft.AspNetCore.SignalR;

namespace cards.Hubs;

public class GameHub : Hub
{
    private readonly ILobbyService _lobbyService;
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILobbyService lobbyService, ILogger<GameHub> logger)
    {
        _lobbyService = lobbyService;
        _logger = logger;
    }

    #region Connections

    public async Task Connect(string username, int lobbyId)
    {
        _logger.LogInformation("{Username} connected to lobby {LobbyId} with connectionId {ConnectionId}", username,
            lobbyId,
            Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());

        _lobbyService.SetConnectionId(lobbyId, username, Context.ConnectionId);

        await SendConnectedUsersUpdateAsync(lobbyId);

        if (_lobbyService.GetLobby(lobbyId).HasStarted)
        {
            _logger.LogInformation("Lobby {LobbyId} has already started, sending game update", lobbyId);
            await SendGameUpdateAsync(lobbyId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var lobbyId = _lobbyService.DisconnectConnection(Context.ConnectionId);
        _logger.LogInformation("Connection {ConnectionId} disconnected from {LobbyId}", Context.ConnectionId, lobbyId);

        if (lobbyId < 0)
        {
            _logger.LogWarning("Connection {ConnectionId} was not connected to any lobby", Context.ConnectionId);
            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
        await SendConnectedUsersUpdateAsync(lobbyId);
        await base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region Receive

    public async Task ReceiveSelectGame(int lobbyId, GameEnum game)
    {
        _lobbyService.GetLobby(lobbyId).SelectGame(game);
        await SendSelectedGameAsync(lobbyId, IGameService.GetTitle(game), IGameService.GetDescription(game));
    }

    public async Task ReceiveStartGame(int lobbyId)
    {
        _logger.LogInformation("Lobby {LobbyId} started a game", lobbyId);
        _lobbyService.GetLobby(lobbyId).StartGame(new DummyInformation());
        await SendGameUpdateAsync(lobbyId);
    }

    public async Task ReceivePlay(int lobbyId, int playerId, int cardIndex)
    {
        _logger.LogDebug("Player {PlayerId} played card {CardIndex}", playerId, cardIndex);
        _lobbyService.GetLobby(lobbyId).Play(playerId, cardIndex);

        if (_lobbyService.GetLobby(lobbyId).HandleWinner())
        {
            await SendRoundWinnerAsync(lobbyId);
        }

        await SendGameUpdateAsync(lobbyId);
    }

    public async Task ReceiveFeature(int lobbyId, int playerId, int featureId)
    {
        _lobbyService.GetLobby(lobbyId).ExecuteFeature(playerId, featureId);
        await SendGameUpdateAsync(lobbyId);
    }

    #endregion

    #region Send

    private async Task SendConnectedUsersUpdateAsync(int lobbyId)
    {
        await Clients.Group(lobbyId.ToString()).SendAsync("ConnectedUsersUpdate",
            _lobbyService.GetLobby(lobbyId).GetConnectedUsernames());
    }

    private async Task SendSelectedGameAsync(int lobbyId, string gameName, string description)
    {
        _logger.LogInformation("Lobby {LobbyId} selected {Game}", lobbyId, gameName);
        await Clients.Group(lobbyId.ToString()).SendAsync("SelectedGameUpdate", gameName, description);
    }

    private async Task SendGameUpdateAsync(int lobbyId)
    {
        _logger.LogDebug("Lobby {LobbyId} received a game update", lobbyId);
        var lobby = _lobbyService.GetLobby(lobbyId);

        var gameData = lobby.GetGameData();

        for (var i = 0; i < gameData.Count; i++)
        {
            var connectionId = lobby.GetConnectionId(i);
            if (connectionId != null)
            {
                gameData[i].Id = i;
                await Clients.Client(connectionId).SendAsync("GameUpdate", gameData[i]);
            }
        }
    }

    private async Task SendRoundWinnerAsync(int lobbyId)
    {
        _logger.LogInformation("Lobby {LobbyId} has finished one round", lobbyId);

        await Clients.Group(lobbyId.ToString())
            .SendAsync("RoundWinner", _lobbyService.GetLobby(lobbyId).GetLeaderboard());
    }

    #endregion
}