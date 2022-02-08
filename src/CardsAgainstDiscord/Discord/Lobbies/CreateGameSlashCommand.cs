using CardsAgainstDiscord.Discord.Commands;
using CardsAgainstDiscord.Services.Contracts;
using Discord;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Lobbies;

public class CreateGameSlashCommand : ISlashCommand
{
    private readonly ILobbiesService _service;

    public CreateGameSlashCommand(ILobbiesService service)
    {
        _service = service;
    }

    public ApplicationCommandProperties Properties => new SlashCommandBuilder
        {
            Name = "create-game",
            Description = "Creates a new game of cards against humanity"
        }
        .Build();

    public async Task HandleInvocationAsync(SocketSlashCommand command)
    {
        if (command.Channel is not SocketGuildChannel channel)
        {
            await command.RespondAsync(embed: EmbedBuilders
                .Error("Sorry, this command can only be used inside a guild.").Build());
            return;
        }

        if (channel is not SocketTextChannel textChannel)
        {
            await command.RespondAsync(embed: EmbedBuilders
                .Error("Okay, how?", "Using commands in voice channels should be straight up illegal.").Build());
            return;
        }

        await command.RespondAsync(
            "A game is being created in this channel.\nYou can now dismiss this message 👌",
            ephemeral: true
        );

        var message = await textChannel.SendMessageAsync("Creating a new game...");

        await _service.CreateLobbyAsync(
            channel.Guild.Id,
            textChannel.Id,
            message.Id,
            command.User.Id
        );
    }
}