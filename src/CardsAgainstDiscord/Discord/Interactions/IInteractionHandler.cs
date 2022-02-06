using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Interactions;

public interface IInteractionHandler
{
    public Task HandleInteractionAsync(SocketInteraction interaction);
}