using CardsAgainstDiscord.Discord;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CardsAgainstDiscord.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordBot(this IServiceCollection services)
    {
        var configuration = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            LogGatewayIntentWarnings = true
        };

        var client = new DiscordSocketClient(configuration);

        services.AddSingleton(client);
        services.AddSingleton(new InteractionService(client));
        services.AddHostedService<DiscordBotService>();

        return services;
    }
}