using CardsAgainstDiscord.Discord;
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

        services.AddTransient(_ => new DiscordSocketClient(configuration));
        services.AddHostedService<DiscordBotService>();
        
        return services;
    }
}