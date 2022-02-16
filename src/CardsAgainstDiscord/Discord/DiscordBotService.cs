using System.Reflection;
using CardsAgainstDiscord.Configuration;
using CardsAgainstDiscord.Exceptions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace CardsAgainstDiscord.Discord;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;

    private readonly InteractionService _interactions;
    
    private readonly DiscordConfiguration _configuration;

    private readonly ILogger<DiscordBotService> _logger;

    private readonly IServiceProvider _services;

    private readonly IHostEnvironment _environment;

    public DiscordBotService(
        DiscordSocketClient client,
        InteractionService interactions,
        IOptions<DiscordConfiguration> configuration,
        ILogger<DiscordBotService> logger,
        IServiceProvider services, 
        IHostEnvironment environment 
    )
    {
        _client = client;
        _interactions = interactions;
        _logger = logger;
        _services = services;
        _environment = environment;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Ready += HandleReadyAsync;
        _client.Log += HandleLogMessageAsync;
        _client.InteractionCreated += HandleInteractionAsync;
        
        await _client.LoginAsync(TokenType.Bot, _configuration.Token);
        await _client.StartAsync();
        await Task.Delay(-1, stoppingToken);
    }

    private async Task HandleReadyAsync()
    {
        await _client.SetStatusAsync(UserStatus.Offline);
        await _client.SetGameAsync("cards against humanity");

        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        if (_environment.IsDevelopment())
        {
            await _interactions.RegisterCommandsToGuildAsync(_configuration.GuildId ?? 0);
            return;
        }

        await _interactions.RegisterCommandsGloballyAsync();
    }

    private Task HandleLogMessageAsync(LogMessage message)
    {
        if (message.Exception != null) _logger.LogCritical("Exception: {exception}", message.Exception);

        _logger.LogInformation("{message}", message.Message);

        return Task.CompletedTask;
    }

    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(_client, interaction);

        try
        {
            await interaction.DeferAsync();
            await _interactions.ExecuteCommandAsync(context, _services);
        }
        catch (Exception exception)
        {
            _logger.LogInformation("Error handling socket interaction: {message}", exception.Message);

            if (exception is EmbeddableException embeddable)
            {
                await interaction.FollowupAsync(
                    ephemeral: true,
                    embed: EmbedBuilders.Error(embeddable.Title, embeddable.Description)
                );
            }
        }
    }
}