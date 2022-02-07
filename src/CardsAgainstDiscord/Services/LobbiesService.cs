using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
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

    private static Embed LobbyCancelledEmbed => new EmbedBuilder
        {
            Title = "Game cancelled by the owner",
            Description = "To create a new game, please use the **/create-game** slash command",
            ThumbnailUrl = DiscordConstants.BannerInactive
        }
        .WithCurrentTimestamp()
        .Build();

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

    public async Task ToggleJoinLobbyAsync(int lobbyId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var lobby = await context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (lobby.JoinedPlayers.Contains(userId))
        {
            lobby.JoinedPlayers.Remove(userId);
        }
        else
        {
            lobby.JoinedPlayers.Add(userId);
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

    private async Task UpdateLobbyEmbedAsync(Lobby lobby, bool cancelled = false)
    {
        var guild = _client.GetGuild(lobby.GuildId);
        var channel = guild.GetTextChannel(lobby.ChannelId);

        if (await channel.GetMessageAsync(lobby.MessageId) is not IUserMessage message)
        {
            // TODO: Log error
            return;
        }

        if (cancelled)
        {
            await message.ModifyAsync(m =>
            {
                m.Content = string.Empty;
                m.Components = Optional<MessageComponent>.Unspecified;
                m.Embed = LobbyCancelledEmbed;
            });
            return;
        }

        var players = lobby.JoinedPlayers.Count == 0
            ? "_No players joined this lobby yet_"
            : string.Join(", ", lobby.JoinedPlayers.Select(id => $"<@!{id}>"));

        var embed = new EmbedBuilder
            {
                Color = DiscordConstants.ColorPrimary,
                Title = "Let's play cards against humanity!",
                Description = "To join or leave this game, click the button below the message.\nYou can leave the game by clicking the button again.",
                ThumbnailUrl = DiscordConstants.Banner
            }
            .AddField("Game owner", $"<@!{lobby.OwnerId}>")
            .AddField("Joined players", players)
            .WithCurrentTimestamp()
            .Build();

        var components = new ComponentBuilder()
            .WithButton(ButtonBuilder.CreatePrimaryButton("👋 Join / leave", $"lobby:join:{lobby.Id}"))
            .WithButton(ButtonBuilder.CreateSecondaryButton("😎 Start", $"lobby:start:{lobby.Id}"))
            .WithButton(ButtonBuilder.CreateSecondaryButton("💀 Cancel", $"lobby:cancel:{lobby.Id}"))
            .Build();

        await message.ModifyAsync(m =>
        {
            m.Content = string.Empty;
            m.Embed = embed;
            m.Components = components;
        });
    }
}