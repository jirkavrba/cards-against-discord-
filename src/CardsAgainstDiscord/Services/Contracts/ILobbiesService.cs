using CardsAgainstDiscord.Model;

namespace CardsAgainstDiscord.Services.Contracts;

public interface ILobbiesService
{
    public Task<Lobby> CreateLobbyAsync(ulong guildId, ulong channelId, ulong messageId, ulong ownerId);
    
    public Task JoinLobbyAsync(Lobby lobby, ulong userId);

    public Task LeaveLobbyAsync(Lobby lobby, ulong userId);

    public Task CancelLobbyAsync(Lobby lobby);
}