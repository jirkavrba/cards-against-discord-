using System.Net.Mime;
using System.Runtime.InteropServices.ObjectiveC;
using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Migrations;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Discord.Games;

public class CardPicksComponentHandler : IComponentHandler
{
    private readonly IDbContextFactory<CardsDbContext> _factory;

    private readonly IGamesService _service;

    public CardPicksComponentHandler(IDbContextFactory<CardsDbContext> factory, IGamesService service)
    {
        _factory = factory;
        _service = service;
    }

    public async Task HandleInteractionAsync(SocketMessageComponent component)
    {
        var id = component.Data.CustomId ?? string.Empty;

        if (!id.StartsWith("game:"))
        {
            return;
        }

        await component.DeferAsync();

        var parts = id.Split(":");
        var action = parts[1];
        var gameId = int.Parse(parts[2]);

        Func<SocketMessageComponent, int, Task> handler = (component.Data.Type, action) switch
        {
            (ComponentType.Button, "pick") => ShowCardsSelection,
            _ => throw new ArgumentOutOfRangeException(nameof(component))
        };

        try
        {
            await handler(component, gameId);
        }
        catch (PlayerNotFoundException)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error("You're not participating in this game").Build(),
                ephemeral: true
            );
        }
    }

    private async Task ShowCardsSelection(SocketMessageComponent component, int gameId)
    {
        var game = await _service.GetPopulatedGameAsync(gameId);
        var player = game.Players.FirstOrDefault(p => p.UserId == component.User.Id)
                     ?? throw new PlayerNotFoundException();

        var cards = player.WhiteCards.ToList();
        var texts = string.Join("\n", cards.Select(c => $" • {c.Text}").ToList());
        var options = cards.Select(c => new SelectMenuOptionBuilder
            {
                // I hope there is not a white card with text over 200 chars...
                Label = c.Text.SafeSubstring(0, 100),
                Description = c.Text.Length >= 100 ? c.Text.SafeSubstring(100, 200) : null,
                Value = c.Id.ToString(),
            })
            .ToList();

        // TODO: Add support for multiple white cards 
        var embed = new EmbedBuilder()
            .WithColor(DiscordConstants.ColorPrimary)
            .WithTitle("Pick a white card to fill in the blanks")
            .WithDescription(game.CurrentRound?.BlackCard.Text.FormatBlackCard(new List<string>()))
            .AddField("Available white cards:", texts)
            .Build();
        
        var select = new ComponentBuilder()
            .WithSelectMenu($"game:pick:{gameId}", options, "Pick your card")
            .Build();

        await component.FollowupAsync(
            ephemeral: true,
            embed: embed,
            components: select
        );
    }
}