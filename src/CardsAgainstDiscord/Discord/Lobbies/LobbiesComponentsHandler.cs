using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Exceptions;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Lobbies;

public class LobbiesComponentsHandler : IComponentHandler
{
    private readonly ILobbiesService _service;

    public LobbiesComponentsHandler(ILobbiesService service)
    {
        _service = service;
    }

    public async Task HandleInteractionAsync(SocketMessageComponent component)
    {
        var id = component.Data.CustomId ?? "";

        // Lobbies can be only interacted with using message buttons
        if (component.Data.Type != ComponentType.Button || !id.StartsWith("lobby:"))
        {
            return;
        }

        await component.DeferAsync();

        // lobby:[action]:[lobby-id]
        var parts = id.Split(":");
        var lobbyId = int.Parse(parts[2]);

        Func<SocketMessageComponent, int, Task> action = parts[1] switch
        {
            "join" => JoinLobby,
            _ => throw new ArgumentOutOfRangeException(nameof(parts)),
        };

        await action(component, lobbyId);
    }

    private async Task JoinLobby(SocketMessageComponent component, int lobbyId)
    {
        try
        {
            await _service.JoinLobbyAsync(lobbyId, component.User.Id);
        }
        catch (LobbyNotFoundException)
        {
            await component.FollowupAsync(embed: EmbedBuilders.Error("Sorry, but this game no longer exists.").Build());
        }
    }
}