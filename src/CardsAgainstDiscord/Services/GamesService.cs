using CardsAgainstDiscord.Configuration;
using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Game = CardsAgainstDiscord.Model.Game;

namespace CardsAgainstDiscord.Services;

public class GamesService : IGamesService
{
    private readonly DiscordSocketClient _client;

    private readonly IDbContextFactory<CardsDbContext> _factory;

    /// <summary>
    /// Number of white cards in each player's hand
    /// </summary>
    private const int HandSize = 8;

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

    public async Task<Game> GetPopulatedGameAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games.Where(g => g.Id == gameId)
            .Include(g => g.CurrentRound)
            .Include(g => g.Players)
            .ThenInclude(p => p.WhiteCards)
            .FirstOrDefaultAsync() ?? throw new GameNotFoundException();

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
            .ToList() // Needed in order to change the context to local evaluation
            .OrderBy(_ => random.Next())
            .First();

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);
        var message = await channel.SendMessageAsync("Starting a new round...");

        game.CurrentRound = new GameRound
        {
            GameId = game.Id,
            MessageId = message.Id,
            Judge = nextJudge,
            BlackCard = selectedBlackCard,
        };

        // Add missing white cards to each player from pool of white cards that have not been used yet
        var availableWhiteCards = context.WhiteCards.Where(c =>
                !context.Rounds.Where(r => r.GameId == game.Id)
                    .Include(r => r.PickedCards)
                    .SelectMany(r => r.PickedCards.Select(p => p.WhiteCardId))
                    .Contains(c.Id)
            )
            .ToList() // Needed in order to change the context to local evaluation
            .OrderBy(_ => random.Next())
            .ToList();

        players.Aggregate(0, (cardsTaken, player) =>
        {
            var missingCards = HandSize - player.WhiteCards.Count;
            
            player.WhiteCards.AddRange(
                availableWhiteCards
                    .Skip(cardsTaken)
                    .Take(missingCards)
            );

            return cardsTaken + missingCards;
        });

        context.Games.Update(game);
        context.Players.UpdateRange(players);

        // Context has to be disposed manually so it can be passed to LINQ lambdas
        await context.SaveChangesAsync();
        await context.DisposeAsync();
        await UpdateGameRoundEmbedAsync(game);
    }

    private async Task UpdateGameRoundEmbedAsync(Game game)
    {
        var round = game.CurrentRound ?? throw new ArgumentNullException(nameof(game.CurrentRound));

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);

        if (await channel.GetMessageAsync(round.MessageId) is not IUserMessage message)
        {
            return;
        }

        // Players that have not submitted the required number of white cards yet
        var playersWithoutPicks = game.Players.Where(p =>
                round.JudgeId != p.Id &&
                round.PickedCards.Count(c => c.PlayerId == p.Id) < round.BlackCard.Picks
            )
            .Select(p => $"<@!{p.UserId}>")
            .ToList();

        var embed = new EmbedBuilder()
            .WithTitle("Waiting for players to pick white cards")
            .WithDescription("To pick white card(s), click the button below this message")
            .WithColor(DiscordConstants.ColorPrimary)
            .WithThumbnailUrl(DiscordConstants.Banner)
            .WithCurrentTimestamp()
            .AddField("Judge", $"<@!{round.Judge.UserId}>")
            .AddField("Selected black card", round.BlackCard.Text.ToBlackCardText());

        if (playersWithoutPicks.Any())
        {
            embed.AddField("Waiting for those players to pick their card(s)", string.Join(", ", playersWithoutPicks));
        }

        var components = new ComponentBuilder().WithButton(
            ButtonBuilder.CreatePrimaryButton("🃏 Pick your card(s)", $"game:pick:{game.Id}")
        );

        await message.ModifyAsync(m =>
        {
            m.Content = "";
            m.Embed = embed.Build();
            m.Components = components.Build();
        });
    }
}