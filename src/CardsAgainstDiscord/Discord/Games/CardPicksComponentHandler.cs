using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Discord.Games;

public class CardPicksComponentHandler : IComponentHandler
{
    private readonly IGamesService _service;

    public CardPicksComponentHandler(IGamesService service)
    {
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
            (ComponentType.SelectMenu, "pick") => PickCard,
            _ => throw new ArgumentOutOfRangeException(nameof(component))
        };

        try
        {
            await handler(component, gameId);
        }
        catch (PlayerIsJudgeException)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error("You are the judge this round. You cannot pick white cards").Build(),
                ephemeral: true
            );
        }
        catch (AlreadyPickedAllWhiteCardsException)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error("You already picked all white cards for this round.").Build(),
                ephemeral: true
            );
        }
        catch (PlayerNotFoundException)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error("You're not participating in this game.").Build(),
                ephemeral: true
            );
        }
    }

    private async Task ShowCardsSelection(SocketMessageComponent component, int gameId)
    {
        var blackCard = await _service.GetCurrentBlackCardAsync(gameId);
        var whiteCards = await _service.GetAvailableWhiteCardsAsync(gameId, component.User.Id);
        var picks = await _service.GetPickedWhiteCardsAsync(gameId, component.User.Id);
        
        var texts = string.Join("\n", whiteCards.Select(c => $" • {c.Text}").ToList());
        var options = whiteCards.Select(c => new SelectMenuOptionBuilder
            {
                // I hope there is not a white card with text over 200 chars...
                Label = c.Text.SafeSubstring(0, 100),
                Description = c.Text.Length >= 100 ? c.Text.SafeSubstring(100, 200) : null,
                Value = c.Id.ToString()
            })
            .ToList();

        var embed = new EmbedBuilder()
            .WithColor(DiscordConstants.ColorPrimary)
            .WithTitle("Pick a white card to fill in the highlighted blank")
            .WithDescription(blackCard.Text.FormatBlackCard(picks.Select(p => p.Text).ToList()))
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

    private async Task PickCard(SocketMessageComponent component, int gameId)
    {
        var playerId = component.User.Id;
        var cardId = int.Parse(component.Data.Values.First());

        var pickAnotherCard = await _service.SubmitPickedCardAsync(gameId, playerId, cardId);

        // If there are more card picks needed
        if (pickAnotherCard)
        {
            await component.DeleteOriginalResponseAsync();
            await ShowCardsSelection(component, gameId);
            return;
        }

        await component.FollowupAsync("All cards picked!\nYou can dismiss the messages now.", ephemeral: true);
    }
}