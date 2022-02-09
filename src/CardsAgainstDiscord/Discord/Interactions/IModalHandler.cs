using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Interactions;

public interface IModalHandler
{
    public Task HandleModalSubmissionAsync(SocketModal modal);
}