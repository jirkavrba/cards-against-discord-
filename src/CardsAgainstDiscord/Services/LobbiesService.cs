using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Services;

public class LobbiesService : ILobbiesService
{
    private readonly DiscordSocketClient _client;
    
    private readonly CardsDbContext _context;

    private readonly IGamesService _gamesService;

    public LobbiesService(CardsDbContext context, DiscordSocketClient client, IGamesService gamesService)
    {
        _client = client;
        _context = context;
        _gamesService = gamesService;
    }

    private static Embed LobbyCancelledEmbed => new EmbedBuilder
        {
            Title = "Game cancelled by the owner",
            Description = "You can create a new game with the **/create-game** command",
            ThumbnailUrl = DiscordConstants.BannerInactive
        }
        .WithCurrentTimestamp()
        .Build();

    public async Task<Lobby> CreateLobbyAsync(ulong guildId, ulong channelId, ulong messageId, ulong ownerId, int winPoints)
    {
        if (winPoints is < 1 or > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(winPoints), "The number of win points must be between 1 and 50");
        }
        
        var lobby = new Lobby
        {
            GuildId = guildId,
            ChannelId = channelId,
            MessageId = messageId,
            OwnerId = ownerId,
            WinPoints = winPoints,
            JoinedPlayers = new List<ulong> {ownerId}
        };

        await _context.Lobbies.AddAsync(lobby);
        await _context.SaveChangesAsync();

        await UpdateLobbyEmbedAsync(lobby);

        return lobby;
    }

    public async Task ToggleJoinLobbyAsync(int lobbyId, ulong userId)
    {
        var lobby = await _context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (lobby.JoinedPlayers.Contains(userId))
            lobby.JoinedPlayers.Remove(userId);
        else
            lobby.JoinedPlayers.Add(userId);

        _context.Lobbies.Update(lobby);

        await UpdateLobbyEmbedAsync(lobby);
        await _context.SaveChangesAsync();
    }

    public async Task CancelLobbyAsync(int lobbyId, ulong userId)
    {
        var lobby = await _context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (lobby.OwnerId != userId) throw new UserIsNotTheOwnerException();

        _context.Lobbies.Remove(lobby);

        await UpdateLobbyEmbedAsync(lobby, true);
        await _context.SaveChangesAsync();
    }

    public async Task StartGameAsync(int lobbyId, ulong userId)
    {
        var lobby = await _context.Lobbies.FindAsync(lobbyId)
                    ?? throw new LobbyNotFoundException();

        if (lobby.OwnerId != userId) throw new UserIsNotTheOwnerException();

        if (lobby.JoinedPlayers.Count < 2) throw new TooFewPlayersException();

        await _gamesService.CreateGameAsync(lobby);
        await UpdateLobbyEmbedAsync(lobby, started: true);
    }

    private async Task UpdateLobbyEmbedAsync(Lobby lobby, bool cancelled = false, bool started = false)
    {
        var guild = _client.GetGuild(lobby.GuildId);
        var channel = guild.GetTextChannel(lobby.ChannelId);

        if (await channel.GetMessageAsync(lobby.MessageId) is not IUserMessage message)
            // TODO: Log error
            return;

        if (started)
        {
            await message.DeleteAsync();
            return;
        }

        if (cancelled)
        {
            await message.ModifyAsync(m =>
            {
                m.Content = string.Empty;
                m.Components = new ComponentBuilder().Build();
                m.Embed = EmbedBuilders.CancelledLobbyEmbed();
            });
            return;
        }

        var embed = EmbedBuilders.LobbyEmbed(lobby.OwnerId, lobby.JoinedPlayers, lobby.WinPoints);
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