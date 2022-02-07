using CardsAgainstDiscord.Discord;
using CardsAgainstDiscord.Discord.Commands;
using CardsAgainstDiscord.Discord.Interactions;
using CardsAgainstDiscord.Discord.Lobbies;
using Discord;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordBot(this IServiceCollection services)
    {
        var configuration = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged,
            LogGatewayIntentWarnings = true,
        };

        services.AddSingleton(new DiscordSocketClient(configuration));
        services.AddHostedService<DiscordBotService>();
        
        return services;
    }

    public static IServiceCollection AddSlashCommands(this IServiceCollection services)
    {
        services.AddTransient<ISlashCommandDispatcher, SlashCommandDispatcher>();
        
        services.AddTransient<ISlashCommand, CreateGameSlashCommand>();
        
        return services;
    }

    public static IServiceCollection AddComponentHandlers(this IServiceCollection services)
    {
        services.AddTransient<IComponentHandler, LobbiesComponentsHandler>();
        
        return services;
    }
}