using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Migrations;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Services;

public class LobbiesService : ILobbiesService
{
    private readonly IDbContextFactory<CardsDbContext> _factory;

    private readonly DiscordSocketClient _client;

    public LobbiesService(IDbContextFactory<CardsDbContext> factory, DiscordSocketClient client)
    {
        _factory = factory;
        _client = client;
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

        await UpdateLobbyEmbedAsync(lobby);

        return lobby;
    }

    public async Task JoinLobbyAsync(int lobbyId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var lobby = await context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (!lobby.JoinedPlayers.Contains(userId))
        {
            lobby.JoinedPlayers.Add(userId);
        }

        context.Lobbies.Update(lobby);

        await UpdateLobbyEmbedAsync(lobby);
        await context.SaveChangesAsync();
    }

    public async Task LeaveLobbyAsync(int lobbyId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        
        var lobby = await context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (lobby.JoinedPlayers.Contains(userId))
        {
            lobby.JoinedPlayers.Remove(userId);
        }

        context.Lobbies.Update(lobby);

        await UpdateLobbyEmbedAsync(lobby);
        await context.SaveChangesAsync();
    }

    public async Task CancelLobbyAsync(int lobbyId)
    {
        await using var context = await _factory.CreateDbContextAsync();
        
        var lobby = await context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        context.Lobbies.Remove(lobby);

        await UpdateLobbyEmbedAsync(lobby);
        await context.SaveChangesAsync();
    }

    private async Task UpdateLobbyEmbedAsync(Lobby lobby)
    {
        var guild = _client.GetGuild(lobby.GuildId);
        var channel = guild.GetTextChannel(lobby.ChannelId);
        var message = await channel.GetMessageAsync(lobby.MessageId);
    }
}