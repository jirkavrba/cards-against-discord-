using System.ComponentModel;
using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Services.Contracts;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Games;

public class JoiningLeavingComponentHandler : IComponentHandler
{
    private readonly IGamesService _service;

    public JoiningLeavingComponentHandler(IGamesService service)
    {
        _service = service;
    }

    public async Task HandleInteractionAsync(SocketMessageComponent component)
    {
        var id = component.Data.CustomId ?? string.Empty;
        
        if (!id.StartsWith("join-game:") && !id.StartsWith("leave-game:")) return;

        await component.DeferAsync();

        var parts = id.Split(":");
        var action = parts[0];
        var gameId = int.Parse(parts[1]);
        var userId = component.User.Id;

        Func<SocketMessageComponent, int, Task> handler = action switch
        {
            "join-game" => JoinGameAsync,
            "leave-game" => LeaveGameAsync,
            _ => throw new ArgumentOutOfRangeException(nameof(component))
        };

        try
        {
            await handler(component, gameId);
        }
        catch (EmbeddableException exception)
        {
            await component.FollowupAsync(
                embed: EmbedBuilders.Error(exception.Title, exception.Description),
                ephemeral: true
            );
        }
    }

    private async Task JoinGameAsync(SocketMessageComponent component, int gameId)
    {
        await _service.JoinGameAsync(gameId, component.User.Id);
        await component.FollowupAsync(ephemeral: true, embed: EmbedBuilders.JoinedGameEmbed());
    }

    private async Task LeaveGameAsync(SocketMessageComponent component, int gameId)
    {
        await _service.LeaveGameAsync(gameId, component.User.Id);
        await component.FollowupAsync(ephemeral: true, embed: EmbedBuilders.LeavedGameAsync());
    }
}