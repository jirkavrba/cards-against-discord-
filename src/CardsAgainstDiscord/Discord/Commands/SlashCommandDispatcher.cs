using CardsAgainstDiscord.Configuration;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace CardsAgainstDiscord.Discord.Commands;

public class SlashCommandDispatcher : ISlashCommandDispatcher
{
    private readonly IEnumerable<ISlashCommand> _commands;

    private readonly DiscordConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public SlashCommandDispatcher(IHostEnvironment environment, IOptions<DiscordConfiguration> configuration,
        IEnumerable<ISlashCommand> commands)
    {
        _environment = environment;
        _commands = commands;
        _configuration = configuration.Value;
    }

    public async Task RegisterCommandsAsync(DiscordSocketClient client)
    {
        client.SlashCommandExecuted += HandleSlashCommandAsync;

        var commands = _commands.Select(c => c.Properties).ToArray();

        // If the bot is running in the development mode, register the commands guild-locally to avoid the 1 hour delay
        // before Discord dispatches global commands update
        if (_environment.IsDevelopment())
        {
            var id = _configuration.GuildId ?? throw new ArgumentNullException(nameof(_configuration.GuildId));
            var guild = client.GetGuild(id) ?? throw new ArgumentException($"Cannot find the configured guild ({id})!");

            await guild.BulkOverwriteApplicationCommandAsync(commands);
            return;
        }

        await client.BulkOverwriteGlobalApplicationCommandsAsync(commands);
    }

    private async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        var handler = _commands.FirstOrDefault(c => c.Properties.Name.GetValueOrDefault() == command.CommandName);

        if (handler == null)
            // TODO: Log warning about missing command handler
            return;

        await handler.HandleInvocationAsync(command);
    }
}