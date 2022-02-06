using Discord;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Discord.Commands;

public interface ISlashCommand
{
    /// <summary>
    /// Application command definition that will get automatically registered upon the service startup
    /// </summary>
    public ApplicationCommandProperties Properties { get; set; } 
    
    /// <summary>
    /// Handle invocation of the command defined id <see cref="Properties"/>
    /// </summary>
    /// <param name="command">Command event data sent by the Discord gateway</param>
    public Task HandleInvocationAsync(SocketSlashCommand command);
}