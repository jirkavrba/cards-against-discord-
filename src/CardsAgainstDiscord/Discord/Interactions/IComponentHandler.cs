using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Interactions;

public interface IComponentHandler
{
    public Task HandleInteractionAsync(SocketMessageComponent component);
}