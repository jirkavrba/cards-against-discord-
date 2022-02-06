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

    public ApplicationCommandProperties Properties => new SlashCommandBuilder()
        {
            Name = "create-game",
            Description = "Creates a new game of cards against humanity"
        }
        .Build();

    public async Task HandleInvocationAsync(SocketSlashCommand command)
    {
        throw new NotImplementedException();
    }
}