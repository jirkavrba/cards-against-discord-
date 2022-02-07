using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Discord.Interactions;
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

        // TODO: Try-catch domain exceptions
        await handler(component, gameId);
    }

    private async Task ShowCardsSelection(SocketMessageComponent component, int gameId)
    {
    }
}