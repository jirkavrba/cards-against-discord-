using System.Xml.Serialization;
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
    /// <summary>
    ///     Number of white cards in each player's hand
    /// </summary>
    private const int HandSize = 8;

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
        await CreateGameRoundAsync(game.Id);

        return game;
    }

    public async Task<BlackCard?> GetCurrentBlackCardAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
            .Include(r => r.BlackCard)
            .FirstOrDefaultAsync(r => r.Id == gameId) ?? throw new GameNotFoundException();

        return game.BlackCard;
    }

    public async Task<List<WhiteCard>> GetAvailableWhiteCardsAsync(int gameId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
            .Include(g => g.BlackCard)
            .Include(g => g.Judge)
            .Include(g => g.Players)
            .ThenInclude(p => p.WhiteCards)
            .Include(g => g.Players)
            .ThenInclude(p => p.PickedCards)
            .ThenInclude(p => p.WhiteCard)
            .FirstOrDefaultAsync(g => g.Id == gameId) ?? throw new GameNotFoundException();

        // The player does not exist within the selected game
        var player = game.Players.FirstOrDefault(p => p.UserId == userId)
                     ?? throw new PlayerNotFoundException();

        // The player is the current judge and therefore cannot pick white cards
        if (game.Judge?.UserId == userId) throw new PlayerIsJudgeException();

        // The player has already selected all white cards 
        if (player.PickedCards.Count >= game.BlackCard?.Picks) throw new AlreadyPickedAllWhiteCardsException();

        return player.WhiteCards.ToList();
    }

    public async Task<List<WhiteCard>> GetPickedWhiteCardsAsync(int gameId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var player = await context.Players
                         .Include(p => p.PickedCards)
                         .ThenInclude(p => p.WhiteCard)
                         .FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == userId)
                     ?? throw new PlayerNotFoundException();

        return player.PickedCards.Select(p => p.WhiteCard).ToList();
    }

    public async Task SelectWhiteCardAsync(int gameId, ulong userId, int whiteCardId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var player = await context.Players.FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == userId)
                     ?? throw new PlayerNotFoundException();

        player.SelectedWhiteCardId = whiteCardId;
        context.Update(player);

        await context.SaveChangesAsync();
    }

    public async Task<bool> ConfirmSelectedCardAsync(int gameId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
                       .Include(g => g.BlackCard)
                       .Include(g => g.Players).ThenInclude(p => p.WhiteCards)
                       .Include(g => g.Players).ThenInclude(p => p.PickedCards)
                       .FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();

        var player = game.Players.FirstOrDefault(p => p.UserId == userId) ?? throw new PlayerNotFoundException();
        var whiteCardId = player.SelectedWhiteCardId ?? throw new NoSelectedWhiteCardException();
        var card = player.WhiteCards.FirstOrDefault(c => c.Id == whiteCardId) ?? throw new WhiteCardNotFoundException();

        // If the player has chosen all the white cards already
        if (player.PickedCards.Count >= game.BlackCard?.Picks) throw new AlreadyPickedAllWhiteCardsException();

        player.WhiteCards.Remove(card);
        player.PickedCards.Add(new PickedCard
        {
            PlayerId = player.Id,
            WhiteCardId = whiteCardId
        });

        context.Players.Update(player);

        await context.SaveChangesAsync();
        await UpdateGameRoundEmbedAsync(gameId);

        // If the player needs to pick more white card(s), early return true
        if (player.PickedCards.Count < game.BlackCard?.Picks) return true;

        // Check, whether all players have submitted all picks
        // There has to be (player - 1 judge) * black picks cards picked
        var pickedCards = game.Players.Aggregate(0, (sum, p) => sum += p.PickedCards.Count);
        if (pickedCards == (game.Players.Count - 1) * game.BlackCard?.Picks)
        {
            await DeleteGameRoundEmbedAsync(gameId);
            await SendJudgeSelectionEmbedAsync(gameId);
        }

        return false;
    }

    public async Task SelectWinnerAsync(int gameId, ulong userId, int playerId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
                       .Include(g => g.Judge)
                       .Include(g => g.Players)
                       .FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();

        if (game.Judge?.UserId != userId) throw new PlayerIsNotJudgeException();

        game.SelectedWinnerId = playerId;

        context.Games.Update(game);
        await context.SaveChangesAsync();
    }

    public async Task ConfirmSelectedWinnerAsync(int gameId, ulong userId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
                       .Include(g => g.Judge)
                       .Include(g => g.Players)
                       .FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();

        if (game.Judge?.UserId != userId) throw new PlayerIsNotJudgeException();
        if (game.SelectedWinnerId == null) throw new NoSelectedSubmissionException();

        var winner = game.Players.FirstOrDefault(p => p.Id == game.SelectedWinnerId)
                     ?? throw new PlayerNotFoundException();

        winner.Score++;
        game.SelectedWinnerId = null;
        
        context.Games.Update(game);
        
        await context.SaveChangesAsync();
        await SendRoundWinnerEmbedAsync(gameId, winner.Id);
        await DeleteGameRoundEmbedAsync(gameId);
        
        await Task.Delay(TimeSpan.FromSeconds(5));
        
        await CreateGameRoundAsync(gameId);
    }

    private async Task CreateGameRoundAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        // Move the judge role to the next player
        var game = await context.Games.Where(g => g.Id == gameId)
            .Include(g => g.UsedBlackCards)
            .Include(g => g.UsedWhiteCards)
            .Include(g => g.Players).ThenInclude(p => p.PickedCards)
            .Include(g => g.Players).ThenInclude(p => p.WhiteCards)
            .FirstOrDefaultAsync() ?? throw new GameNotFoundException();

        var players = game.Players;
        var previousJudge = players.FindIndex(p => p.Id == game.JudgeId);
        var nextJudge = players[(previousJudge + 1) % players.Count];

        // If there is a black card from the previous round, add it to the used black cards collection
        if (game.BlackCard != null)
        {
            game.UsedBlackCards.Add(game.BlackCard);
        }

        // Similarly, if there are submitted white cards from the previous round, add the to the used white cards collection
        foreach (var player in game.Players)
        {
            // TODO: adding to used white cards breaks the transaction for some reason
            // game.UsedWhiteCards.AddRange(player.PickedCards.Select(p => p.WhiteCard));
            player.PickedCards.Clear();
        }

        // Select a card randomly from all black cards that have not been used yet in this game
        var random = new Random();
        var selectedBlackCard = context.BlackCards.Where(c => !game.UsedBlackCards.Contains(c))
            .ToList() // Needed in order to change the context to local evaluation
            .OrderBy(_ => random.Next())
            .First();

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);
        var message = await channel.SendMessageAsync("Starting a new round...");

        game.JudgeId = nextJudge.Id;
        game.BlackCardId = selectedBlackCard.Id;
        game.MessageId = message.Id;

        // Add missing white cards to each player from pool of white cards that have not been used yet
        var availableWhiteCards = context.WhiteCards.Where(c => !game.UsedWhiteCards.Contains(c))
            .ToList() // Needed in order to change the context to local evaluation
            .OrderBy(_ => random.Next())
            .ToList();

        // TODO: Maybe write this in a cleaner, more functional approach?
        var takenCards = 0;

        foreach (var player in game.Players)
        {
            var missingCards = HandSize - player.WhiteCards.Count;

            player.WhiteCards.AddRange(availableWhiteCards.Skip(takenCards).Take(missingCards));

            takenCards += missingCards;
        }

        context.Games.Update(game);

        await context.SaveChangesAsync();
        await UpdateGameRoundEmbedAsync(game);
    }

    private async Task UpdateGameRoundEmbedAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
            .Include(g => g.Judge)
            .Include(g => g.BlackCard)
            .Include(g => g.Players).ThenInclude(p => p.PickedCards)
            .FirstOrDefaultAsync(g => g.Id == gameId) ?? throw new GameNotFoundException();

        await UpdateGameRoundEmbedAsync(game);
    }

    private async Task UpdateGameRoundEmbedAsync(Game game)
    {
        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);

        if (await channel.GetMessageAsync(game.MessageId ?? 0) is not IUserMessage message) return;

        var text = game.BlackCard!.FormattedText;
        var picks = game.BlackCard.Picks;
        var judge = game.Judge!.UserId;
        var players = game.Players.Where(p => p.Id != game.JudgeId)
            .Select(p => p.PickedCards.Count < picks
                ? $"⏳ {p.UserId.AsUserMention()}"
                : $"✅ {p.UserId.AsUserMention()}"
            )
            .OrderBy(p => p)
            .ToList();

        var embed = EmbedBuilders.GameRoundEmbed(text, judge, players);
        var components = new ComponentBuilder().WithButton(
            ButtonBuilder.CreatePrimaryButton(
                "Pick your card" + (picks > 1 ? "s" : string.Empty),
                $"game:pick:{game.Id}"
            )
        ).Build();

        await message.ModifyAsync(m =>
        {
            m.Content = "";
            m.Embed = embed;
            m.Components = components;
        });
    }

    private async Task DeleteGameRoundEmbedAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games.FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();

        if (game.MessageId == null) return;

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);

        await channel.DeleteMessageAsync(game.MessageId.Value);

        game.MessageId = null;
        context.Games.Update(game);

        await context.SaveChangesAsync();
    }

    private async Task SendJudgeSelectionEmbedAsync(int gameId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
                       .Include(g => g.BlackCard)
                       .Include(g => g.Judge)
                       .Include(g => g.Players)
                       .ThenInclude(p => p.PickedCards)
                       .ThenInclude(c => c.WhiteCard)
                       .FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);

        var random = new Random();
        var submissions = game.Players
            .Where(p => p.Id != game.JudgeId)
            .Select(p =>
            {
                var id = p.Id;
                var cards = p.PickedCards.Select(c => c.WhiteCard.Text).ToList();
                var text = game.BlackCard!.Text.FormatBlackCard(cards);

                return (id, cards, text);
            })
            .OrderBy(_ => random.Next())
            .ToList();

        var embed = EmbedBuilders.JudgeSelectionEmbed(game.Judge!.UserId, submissions.Select((s, i) => s.text));

        var options = submissions.Select((s, i) => new SelectMenuOptionBuilder(
            $"{i + 1}: {string.Join(", ", s.cards)}".SafeSubstring(0, 100),
            s.id.ToString())
        ).ToList();

        var components = new ComponentBuilder()
            .WithSelectMenu($"game:judge:{gameId}", options, "Select the winner entry")
            .WithButton("Confirm choice", $"game:confirm-judge:{gameId}")
            .Build();

        var message = await channel.SendMessageAsync(embed: embed, components: components);

        game.MessageId = message.Id;

        await context.SaveChangesAsync();
    }

    private async Task SendRoundWinnerEmbedAsync(int gameId, int winnerId)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = await context.Games
                       .Include(g => g.BlackCard)
                       .Include(g => g.Judge)
                       .Include(g => g.Players)
                       .ThenInclude(p => p.PickedCards)
                       .ThenInclude(c => c.WhiteCard)
                       .FirstOrDefaultAsync(g => g.Id == gameId)
                   ?? throw new GameNotFoundException();
        
        var winner = game.Players.FirstOrDefault(p => p.Id == winnerId) ?? throw new PlayerNotFoundException();
        var submission = winner.PickedCards.Select(c => c.WhiteCard.Text).ToList();
        
        var text = game.BlackCard?.Text.FormatBlackCard(submission);
        var scoreBoard = game.Players
            .OrderByDescending(p => p.Score)
            .Select(p => $"{p.UserId.AsUserMention()} - **{p.Score} points**")
            .ToList();
        
        var embed = EmbedBuilders.WinnerEmbed(text!, winner.UserId, scoreBoard);

        var guild = _client.GetGuild(game.GuildId);
        var channel = guild.GetTextChannel(game.ChannelId);

        await channel.SendMessageAsync(embed: embed);
    }
}