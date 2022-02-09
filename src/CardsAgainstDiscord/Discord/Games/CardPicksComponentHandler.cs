using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

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

        if (!id.StartsWith("game:")) return;

        await component.DeferAsync();

        var parts = id.Split(":");
        var action = parts[1];
        var gameId = int.Parse(parts[2]);

        Func<SocketMessageComponent, int, Task> handler = (component.Data.Type, action) switch
        {
            (ComponentType.Button, "pick") => ShowCardsSelectionAsync,
            (ComponentType.Button, "confirm-pick") => ConfirmSelectedCardAsync,
            (ComponentType.SelectMenu, "pick") => SelectCardAsync,
            // (ComponentType.SelectMenu, "judge") => SelectWinner,
            _ => throw new ArgumentOutOfRangeException(nameof(component))
        };

        try
        {
            await handler(component, gameId);
        }
        catch (EmbeddableException exception)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error(exception.Title, exception.Description).Build(),
                ephemeral: true
            );
        }
    }

    private async Task SelectCardAsync(SocketMessageComponent component, int gameId)
    {
        var whiteCardId = int.Parse(component.Data.Values.First());
        var userId = component.User.Id;

        await _service.SelectWhiteCardAsync(gameId, userId, whiteCardId);
    }

    private async Task ConfirmSelectedCardAsync(SocketMessageComponent component, int gameId)
    {
        var userId = component.User.Id;
        var shouldPickAnother = await _service.ConfirmSelectedCardAsync(gameId, userId);

        if (shouldPickAnother)
        {
            await ShowCardsSelectionAsync(component, gameId, true);
            return;
        }

        await component.ModifyOriginalResponseAsync(m =>
        {
            m.Embed = EmbedBuilders.AllWhiteCardsPickedEmbed();
            m.Components = new ComponentBuilder().Build();
        });
    }

    private async Task ShowCardsSelectionAsync(SocketInteraction button, int gameId) =>
        await ShowCardsSelectionAsync(button, gameId, false);

    private async Task ShowCardsSelectionAsync(SocketInteraction button, int gameId, bool update)
    {
        var blackCard = await _service.GetCurrentBlackCardAsync(gameId);
        var pickedCards = await _service.GetPickedWhiteCardsAsync(gameId, button.User.Id);
        var whiteCards = await _service.GetAvailableWhiteCardsAsync(gameId, button.User.Id);

        var embed = EmbedBuilders.WhiteCardSelectionEmbed(
            blackCard!.Text,
            pickedCards.Select(c => c.Text).ToList(),
            whiteCards.Select(c => c.Text).ToList()
        );

        var options = whiteCards.Select(c =>
            new SelectMenuOptionBuilder()
                .WithLabel(c.Text.SafeSubstring(0, 100))
                .WithValue(c.Id.ToString())
        );

        var components = new ComponentBuilder()
            .WithSelectMenu($"game:pick:{gameId}", options.ToList(), "Select a white card")
            .WithButton(ButtonBuilder.CreatePrimaryButton("Confirm pick", $"game:confirm-pick:{gameId}"))
            .Build();

        if (update)
        {
            await button.ModifyOriginalResponseAsync(m =>
            {
                m.Embed = embed;
                m.Components = components;
            });
            return;
        }

        await button.FollowupAsync(ephemeral: true, embed: embed, components: components);
    }
}