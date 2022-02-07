using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Services;

public class GamesService : IGamesService
{
    private readonly DiscordSocketClient _client;

    private readonly IDbContextFactory<CardsDbContext> _factory;

    public GamesService(DiscordSocketClient client, IDbContextFactory<CardsDbContext> factory)
    {
        _client = client;
        _factory = factory;
    }

    public async Task<Game> CreateGameAsync(Lobby lobby)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = new Game
        {
            GuildId = lobby.GuildId,
            ChannelId = lobby.ChannelId,
            Players = lobby.JoinedPlayers.Select(id => new Player {UserId = id}).ToList()
        };

        context.Games.Add(game);
        context.Lobbies.Remove(lobby);

        await context.SaveChangesAsync();
        await CreateGameRound(game);

        return game;
    }

    private async Task CreateGameRound(Game game)
    {
        var context = await _factory.CreateDbContextAsync();

        // Move the judge role to the next player
        var players = game.Players;
        var previousJudge = players.FindIndex(p => p.Id == game.CurrentRound?.JudgeId);
        var nextJudge = players[(previousJudge + 1) % players.Count];

        // Select a card randomly from all black cards that have not been used yet in this game
        var random = new Random();
        var selectedBlackCard = context.BlackCards.Where(c =>
                !context.Rounds.Where(r => r.GameId == game.Id)
                    .Select(r => r.BlackCardId)
                    .Contains(c.Id)
            )
            .Select(c => c.Id)
            .ToList() // Needed in order to change the context to local evaluation
            .OrderBy(_ => random.Next())
            .First();

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);
        var message = await channel.SendMessageAsync("Starting a new round...");

        game.CurrentRound = new GameRound
        {
            GameId = game.Id,
            JudgeId = nextJudge.Id,
            BlackCardId = selectedBlackCard,
            MessageId = message.Id
        };
        
        // TODO: Add missing white cards to every player's hand

        context.Games.Update(game);

        // Context has to be disposed manually so it can be passed to LINQ lambdas
        await context.SaveChangesAsync();
        await context.DisposeAsync();
        
        // TODO: Update game round embed
    }
}