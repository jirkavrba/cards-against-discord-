using CardsAgainstDiscord.Services.Contracts;
using Discord.Interactions;

namespace CardsAgainstDiscord.Discord.Modules;

public class LobbiesModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILobbiesService _service;

    public LobbiesModule(ILobbiesService service)
    {
        _service = service;
    }

    [RequireContext(ContextType.Guild)]
    [SlashCommand("game", "Create a new cards against humanity game")]
    public async Task CreateLobbyCommandAsync(
        [Summary("win-points", "Number of points required for win (defaults to 10)")] 
        int points = 10
    )
    {
        var message = await Context.Channel.SendMessageAsync("Creating a new game...");

        try
        {
            await Context.Interaction.RespondAsync("A game is being created in this channel 👌", ephemeral: true);
            await _service.CreateLobbyAsync(Context.Guild.Id, Context.Channel.Id, message.Id, Context.User.Id, points);
        }
        catch (Exception exception)
        {
            await Context.Interaction.RespondAsync(
                embed: EmbedBuilders.Error("Sorry there was an issue", exception.Message),
                ephemeral: true
            );
        }
    }
}