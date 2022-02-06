using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Commands;

public interface ISlashCommandDispatcher
{
    public Task RegisterCommandsAsync(DiscordSocketClient client);
}