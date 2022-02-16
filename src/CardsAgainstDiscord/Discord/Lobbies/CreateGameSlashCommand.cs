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
            Name = "game",
            Description = "Starts a new game of cards against humanity",
            Options = new List<SlashCommandOptionBuilder>
            {
                new()
                {
                    Name = "win-points",
                    Description = "Number of points required to win (defaults to 10)",
                    IsRequired = false,
                    Type = ApplicationCommandOptionType.Integer
                }
            }
        }
        .Build();

    public async Task HandleInvocationAsync(SocketSlashCommand command)
    {
        if (command.Channel is not SocketGuildChannel channel)
        {
            await command.RespondAsync(embed: EmbedBuilders.Error("Sorry, this command can only be used inside a guild."));
            return;
        }

        if (channel is not SocketTextChannel textChannel)
        {
            await command.RespondAsync(embed: EmbedBuilders
                .Error("Okay, how?", "Using commands in voice channels should be straight up illegal."));
            return;
        }

    }
}