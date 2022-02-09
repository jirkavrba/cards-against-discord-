using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Extensions;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
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
            (ComponentType.SelectMenu, "pick") => SelectCardAsync,
            (ComponentType.Button, "confirm-pick") => ConfirmSelectedCardAsync,
            (ComponentType.SelectMenu, "judge") => SelectWinnerAsync,
            (ComponentType.Button, "confirm-judge") => ConfirmSelectedWinnerAsync,
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

    private async Task SelectCardAsync(SocketMessageComponent menu, int gameId)
    {
        var whiteCardId = int.Parse(menu.Data.Values.First());
        var userId = menu.User.Id;

        await _service.SelectWhiteCardAsync(gameId, userId, whiteCardId);
    }

    private async Task ConfirmSelectedCardAsync(SocketMessageComponent button, int gameId)
    {
        var userId = button.User.Id;
        var shouldPickAnother = await _service.ConfirmSelectedCardAsync(gameId, userId);

        if (shouldPickAnother)
        {
            await ShowCardsSelectionAsync(button, gameId, true);
            return;
        }

        await button.ModifyOriginalResponseAsync(m =>
        {
            m.Content = "👌";
            m.Embed = null;
            m.Components = new ComponentBuilder().Build();
        });
    }

    private async Task SelectWinnerAsync(SocketMessageComponent menu, int gameId)
    {
        var playerId = int.Parse(menu.Data.Values.First());
        var userId = menu.User.Id;

        await _service.SelectWinnerAsync(gameId, userId, playerId);
    }

    private async Task ConfirmSelectedWinnerAsync(SocketMessageComponent button, int gameId)
    {
        var userId = button.User.Id;

        await _service.ConfirmSelectedWinnerAsync(gameId, userId);
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