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
            (ComponentType.Button, "pick") => ShowCardsSelection,
            (ComponentType.SelectMenu, "pick") => PickCard,
            (ComponentType.SelectMenu, "judge") => SelectWinner,
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

    private async Task ShowCardsSelection(SocketMessageComponent component, int gameId)
    {
        await ShowCardsSelection(component, gameId, false);
    }

    private async Task ShowCardsSelection(SocketMessageComponent component, int gameId, bool repeated)
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
            .WithDescription(blackCard.FormattedText)
            .AddField("Available white cards:", texts)
            .Build();

        var select = new ComponentBuilder()
            .WithSelectMenu($"game:pick:{gameId}", options, "Pick your card")
            .Build();

        if (repeated)
        {
            var original = await component.GetOriginalResponseAsync();

            await original.ModifyAsync(m =>
            {
                m.Embed = embed;
                m.Components = select;
            });
            return;
        }

        await component.FollowupAsync(ephemeral: true, embed: embed, components: select);
    }

    private async Task PickCard(SocketMessageComponent component, int gameId)
    {
        var playerId = component.User.Id;
        var cardId = int.Parse(component.Data.Values.First());

        var pickAnotherCard = await _service.SubmitPickedCardAsync(gameId, playerId, cardId);

        // If there are more card picks needed
        if (pickAnotherCard)
        {
            await ShowCardsSelection(component, gameId, true);
            return;
        }

        var original = await component.GetOriginalResponseAsync();
        var embed = new EmbedBuilder()
            .WithColor(DiscordConstants.ColorGreen)
            .WithTitle("All cards picked!")
            .WithDescription("Now wait for the other players to make their choice.")
            .WithCurrentTimestamp();

        await original.ModifyAsync(m =>
        {
            m.Embed = embed.Build();
            m.Components = new ComponentBuilder().Build();
        });
    }

    private async Task SelectWinner(SocketMessageComponent component, int gameId)
    {
        var playerId = component.User.Id;
        var winnerId = int.Parse(component.Data.Values.First());

        var (winner, submission) = await _service.SubmitWinnerAsync(gameId, playerId, winnerId);

        var embed = new EmbedBuilder()
            .WithTitle("The judge picked this submission as the winner")
            .WithDescription(submission)
            .WithColor(DiscordConstants.ColorGreen)
            .WithCurrentTimestamp()
            .AddField("Submitted by", $"<@!{winner.UserId}>")
            .Build();

        await component.Message.DeleteAsync();
        await component.FollowupAsync(embed: embed);

        await _service.CreateGameRoundAsync(gameId);
    }
}