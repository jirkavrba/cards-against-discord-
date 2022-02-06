using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Services;

public class LobbiesService : ILobbiesService
{
    private readonly IDbContextFactory<CardsDbContext> _factory;

    public LobbiesService(IDbContextFactory<CardsDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Lobby> CreateLobbyAsync(ulong guildId, ulong channelId, ulong messageId, ulong ownerId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var lobby = new Lobby
        {
            GuildId = guildId,
            ChannelId = channelId,
            MessageId = messageId,
            OwnerId = ownerId,
            JoinedPlayers = new List<ulong> {ownerId}
        };

        await context.Lobbies.AddAsync(lobby);
        await context.SaveChangesAsync();

        return lobby;
    }

    public Task JoinLobbyAsync(Lobby lobby, ulong userId)
    {
        throw new NotImplementedException();
    }

    public Task LeaveLobbyAsync(Lobby lobby, ulong userId)
    {
        throw new NotImplementedException();
    }

    public Task CancelLobbyAsync(Lobby lobby)
    {
        throw new NotImplementedException();
    }
}