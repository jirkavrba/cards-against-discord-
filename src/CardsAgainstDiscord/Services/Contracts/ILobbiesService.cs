using CardsAgainstDiscord.Model;

namespace CardsAgainstDiscord.Services.Contracts;

public interface ILobbiesService
{
    public Task<Lobby> CreateLobbyAsync(ulong guildId, ulong channelId, ulong messageId, ulong ownerId);
    
    public Task JoinLobbyAsync(int lobbyId, ulong userId);

    public Task LeaveLobbyAsync(int lobbyId, ulong userId);

    public Task CancelLobbyAsync(int lobbyId);
}