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
            "start" => StartGame,
            "cancel" => CancelGame,
            _ => throw new ArgumentOutOfRangeException(nameof(parts)),
        };

        try
        {
            await action(component, lobbyId);
        }
        catch (LobbyNotFoundException)
        {
            await component.FollowupAsync(
                ephemeral: true,
                embed: EmbedBuilders.Error(
                    "Sorry, but this game no longer exists.",
                    "You can create a new game with the **/create-game** command"
                ).Build()
            );
        }
        catch (UserIsNotLobbyOwnerException)
        {
            await component.FollowupAsync(
                ephemeral: true,
                embed: EmbedBuilders.Error(
                    "Sorry, but only the game owner is allowed to do that",
                    "You can create a new game with the **/create-game** command"
                ).Build()
            );
        }
    }

    private async Task JoinLobby(SocketMessageComponent component, int lobbyId)
    {
        await _service.ToggleJoinLobbyAsync(lobbyId, component.User.Id);
    }

    private async Task StartGame(SocketMessageComponent component, int lobbyId)
    {
    }

    private async Task CancelGame(SocketMessageComponent component, int lobbyId)
    {
        await _service.CancelLobbyAsync(lobbyId, component.User.Id);
    }
}