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
        await CreateGameRound(game);

        return game;
    }

    public async Task<BlackCard> GetCurrentBlackCardAsync(int gameId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var round = await context.Rounds
        //     .Include(r => r.BlackCard)
        //     .FirstOrDefaultAsync(r => r.GameId == gameId) ?? throw new GameNotFoundException();
        //
        // return round.BlackCard;
        return null!;
    }

    public async Task<List<WhiteCard>> GetAvailableWhiteCardsAsync(int gameId, ulong userId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var round = await context.Rounds
        //                 .Include(r => r.BlackCard)
        //                 .FirstOrDefaultAsync(r => r.GameId == gameId)
        //             ?? throw new GameNotFoundException();
        //
        // var player = await context.Players
        //                  .Include(p => p.WhiteCards)
        //                  .Include(p => p.PickedCards)
        //                  .FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == userId)
        //              ?? throw new PlayerNotFoundException();
        //
        // // Do not allow judge to pick white cards
        // if (round.JudgeId == player.Id) throw new PlayerIsJudgeException();
        //
        // // Do not allow users to pick more than <black card picks> cards
        // if (player.PickedCards.Count(p => p.RoundId == round.Id) >= round.BlackCard.Picks)
        //     throw new AlreadyPickedAllWhiteCardsException();
        //
        // return player.WhiteCards;
        return null!;
    }

    public async Task<List<WhiteCard>> GetPickedWhiteCardsAsync(int gameId, ulong userId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var player = await context.Players
        //                  .Include(p => p.PickedCards)
        //                  .ThenInclude(p => p.WhiteCard)
        //                  .FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == userId)
        //              ?? throw new PlayerNotFoundException();
        //
        // return player.PickedCards.Select(p => p.WhiteCard).ToList();
        return null!;
    }

    public async Task<bool> SubmitPickedCardAsync(int gameId, ulong playerId, int cardId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var round = await context.Rounds
        //     .Include(r => r.BlackCard)
        //     .FirstOrDefaultAsync(r => r.GameId == gameId) ?? throw new GameNotFoundException();
        //
        // var player = await context.Players
        //                  .Include(p => p.PickedCards)
        //                  .Include(p => p.WhiteCards)
        //                  .FirstOrDefaultAsync(p => p.GameId == gameId && p.UserId == playerId) ??
        //              throw new PlayerNotFoundException();
        //
        // if (player.PickedCards.Count(p => p.RoundId == round.Id) >= round.BlackCard.Picks)
        //     throw new AlreadyPickedAllWhiteCardsException();
        //
        // var card = player.WhiteCards.First(c => c.Id == cardId);
        // var pick = new PickedCard
        // {
        //     RoundId = round.Id,
        //     PlayerId = player.Id,
        //     WhiteCardId = cardId
        // };
        //
        // round.PickedCards.Add(pick);
        // player.WhiteCards.Remove(card);
        //
        // context.Rounds.Update(round);
        // context.Players.Update(player);
        //
        // await context.SaveChangesAsync();
        // await UpdateGameRoundEmbedAsync(gameId);
        //
        //
        // // If the player needs to pick more cards, early return true
        // if (round.PickedCards.Count(r => r.PlayerId == player.Id) < round.BlackCard.Picks) return true;
        //
        // // Check, whether all players have submitted all picks
        // // There has to be (player - 1 judge) * black picks cards picked
        // var players = await context.Players.CountAsync(p => p.GameId == gameId);
        //
        // if (round.PickedCards.Count == (players - 1) * round.BlackCard.Picks)
        // {
        //     // List all submitted cards in a random order for the judge to pick and delete the original message
        //     await UpdateGameRoundEmbedAsync(gameId);
        //     await SendJudgeSelectionEmbedAsync(gameId);
        // }
        //
        return false;
    }

    public async Task CreateGameRound(int gameId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var game = await context.Games
        //                .Include(g => g.CurrentRound)
        //                .Include(g => g.Players)
        //                .ThenInclude(p => p.WhiteCards)
        //                .FirstOrDefaultAsync(g => g.Id == gameId)
        //            ?? throw new GameNotFoundException();
        //
        // await CreateGameRound(game);
    }

    public async Task<(Player, string)> SubmitWinnerAsync(int gameId, ulong playerId, int winnerId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var game = await context.Games
        //     .Include(g => g.Players)
        //     .Include(g => g.CurrentRound).ThenInclude(p => p.Judge)
        //     .Include(g => g.CurrentRound).ThenInclude(p => p.BlackCard)
        //     .Include(g => g.CurrentRound)
        //     .ThenInclude(p => p.PickedCards)
        //     .ThenInclude(c => c.WhiteCard)
        //     .FirstOrDefaultAsync(g => g.Id == gameId) ?? throw new GameNotFoundException();
        //
        // if (playerId != game.CurrentRound.Judge.UserId) throw new PlayerIsNotJudgeException();
        //
        // var winner = game.Players.FirstOrDefault(p => p.Id == winnerId)
        //              ?? throw new PlayerNotFoundException();
        //
        // var cards = game.CurrentRound.PickedCards.Where(p => p.PlayerId == winnerId)
        //     .Select(c => c.WhiteCard.Text)
        //     .ToList();
        //
        // var submission = game.CurrentRound.BlackCard.Text.FormatBlackCard(cards);
        //
        // // TODO: Update score
        //
        // await CreateGameRound(game);
        //
        // return (winner, submission);
        return (null!, null!);
    }

    private async Task CreateGameRound(Game game)
    {
        // var context = await _factory.CreateDbContextAsync();
        //
        // // Move the judge role to the next player
        // var players = game.Players;
        // var previousJudge = players.FindIndex(p => p.Id == game.CurrentRound?.JudgeId);
        // var nextJudge = players[(previousJudge + 1) % players.Count];
        //
        // // Select a card randomly from all black cards that have not been used yet in this game
        // var random = new Random();
        // var selectedBlackCard = context.BlackCards.Where(c =>
        //         !context.Rounds.Where(r => r.GameId == game.Id)
        //             .Select(r => r.BlackCardId)
        //             .Contains(c.Id)
        //     )
        //     .ToList() // Needed in order to change the context to local evaluation
        //     .OrderBy(_ => random.Next())
        //     .First();
        //
        // var guild = _client.GetGuild(game.GuildId);
        // var channel = guild.GetTextChannel(game.ChannelId);
        // var message = await channel.SendMessageAsync("Starting a new round...");
        //
        // game.CurrentRound = new GameRound
        // {
        //     GameId = game.Id,
        //     MessageId = message.Id,
        //     Judge = nextJudge,
        //     BlackCard = selectedBlackCard
        // };
        //
        // // Add missing white cards to each player from pool of white cards that have not been used yet
        // var availableWhiteCards = context.WhiteCards.Where(c =>
        //         !context.Rounds.Where(r => r.GameId == game.Id)
        //             .Include(r => r.PickedCards)
        //             .SelectMany(r => r.PickedCards.Select(p => p.WhiteCardId))
        //             .Contains(c.Id)
        //     )
        //     .ToList() // Needed in order to change the context to local evaluation
        //     .OrderBy(_ => random.Next())
        //     .ToList();
        //
        // players.Aggregate(0, (cardsTaken, player) =>
        // {
        //     var missingCards = HandSize - player.WhiteCards.Count;
        //
        //     player.WhiteCards.AddRange(
        //         availableWhiteCards
        //             .Skip(cardsTaken)
        //             .Take(missingCards)
        //     );
        //
        //     return cardsTaken + missingCards;
        // });
        //
        // context.Games.Update(game);
        // context.Players.UpdateRange(players);
        //
        // // Context has to be disposed manually so it can be passed to LINQ lambdas
        // await context.SaveChangesAsync();
        // await context.DisposeAsync();
        // await UpdateGameRoundEmbedAsync(game);
    }

    private async Task UpdateGameRoundEmbedAsync(int gameId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var game = await context.Games
        //     .Include(g => g.Players)
        //     .Include(g => g.CurrentRound).ThenInclude(r => r!.PickedCards)
        //     .Include(g => g.CurrentRound).ThenInclude(r => r!.BlackCard)
        //     .Include(g => g.CurrentRound).ThenInclude(r => r!.Judge)
        //     .Where(g => g.Id == gameId)
        //     .FirstOrDefaultAsync() ?? throw new GameNotFoundException();
        //
        // await UpdateGameRoundEmbedAsync(game);
    }

    private async Task UpdateGameRoundEmbedAsync(Game game)
    {
        // var round = game.CurrentRound ?? throw new ArgumentNullException(nameof(game.CurrentRound));
        //
        // var guild = _client.GetGuild(game.GuildId);
        // var channel = guild.GetTextChannel(game.ChannelId);
        //
        // if (await channel.GetMessageAsync(round.MessageId) is not IUserMessage message) return;
        //
        // var players = game.Players
        //     .Where(p => p.Id != round.JudgeId)
        //     .Select(p =>
        //     {
        //         var mention = $"<@!{p.UserId}>";
        //
        //         return round.PickedCards.Count(c => c.PlayerId == p.Id) < round.BlackCard.Picks
        //             ? $"⏳ {mention} - Choosing white cards"
        //             : $"✅ {mention} - Done";
        //     })
        //     .OrderBy(p => p);
        //
        // var embed = new EmbedBuilder()
        //     .WithTitle("Waiting for players to pick white cards")
        //     .WithDescription("To pick white card(s), click the button below this message")
        //     .WithColor(DiscordConstants.ColorPrimary)
        //     .WithThumbnailUrl(DiscordConstants.Banner)
        //     .WithCurrentTimestamp()
        //     .AddField("Judge", $"<@!{round.Judge.UserId}>")
        //     .AddField("Players", string.Join("\n", players))
        //     .AddField("Selected black card", round.BlackCard.Text.FormatBlackCard());
        //
        // var components = new ComponentBuilder().WithButton(
        //     ButtonBuilder.CreatePrimaryButton("🃏 Pick your card(s)", $"game:pick:{game.Id}")
        // );
        //
        // await message.ModifyAsync(m =>
        // {
        //     m.Content = "";
        //     m.Embed = embed.Build();
        //     m.Components = components.Build();
        // });
    }

    private async Task SendJudgeSelectionEmbedAsync(int gameId)
    {
        // await using var context = await _factory.CreateDbContextAsync();
        //
        // var game = await context.Games
        //     .Include(g => g.CurrentRound).ThenInclude(r => r!.Judge)
        //     .Include(g => g.CurrentRound).ThenInclude(r => r!.BlackCard)
        //     .Include(g => g.CurrentRound)
        //     .ThenInclude(r => r!.PickedCards)
        //     .ThenInclude(p => p.WhiteCard)
        //     .FirstOrDefaultAsync(g => g.Id == gameId) ?? throw new GameNotFoundException();
        //
        // var guild = _client.GetGuild(game.GuildId);
        // var channel = guild.GetTextChannel(game.ChannelId);
        //
        // if (await channel.GetMessageAsync(game.CurrentRound!.MessageId) is not IUserMessage message) return;
        //
        // var blackCard = game.CurrentRound.BlackCard.Text;
        //
        // var random = new Random();
        // var submissions = game.CurrentRound.PickedCards
        //     .GroupBy(c => c.PlayerId)
        //     .OrderBy(_ => random.Next())
        //     .ToList();
        //
        // var texts = submissions.Select((cards, i) =>
        //     $"**{i}.** " + blackCard
        //         .FormatBlackCard(cards.Select(c => c.WhiteCard.Text)
        //             .ToList())
        // ).ToList();
        //
        // var embed = new EmbedBuilder()
        //     .WithTitle("Waiting for the judge to select his favorite submission")
        //     .WithDescription("To make the choice, use the button below this message")
        //     .WithColor(DiscordConstants.ColorPrimary)
        //     .WithCurrentTimestamp()
        //     .AddField("Judge", $"<@!{game.CurrentRound!.Judge.UserId}>")
        //     .AddField("Submissions", string.Join("\n\n", texts))
        //     .Build();
        //
        // var components = new ComponentBuilder()
        //     .WithSelectMenu(
        //         $"game:judge:{game.CurrentRound.Id}",
        //         submissions.Select((t, i) => new SelectMenuOptionBuilder
        //         {
        //             Label = $"{i + 1}. " + string.Join(", ", t.Select(c => c.WhiteCard.Text)),
        //             Value = t.Key.ToString()
        //         }).ToList()
        //     )
        //     .Build();
        //
        // await message.DeleteAsync();
        // await channel.SendMessageAsync(
        //     embed: embed,
        //     components: components
        // );
    }
}