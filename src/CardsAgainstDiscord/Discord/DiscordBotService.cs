using CardsAgainstDiscord.Configuration;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace CardsAgainstDiscord.Discord;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;
    
    private readonly DiscordConfiguration _configuration;

    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(DiscordSocketClient client, IOptions<DiscordConfiguration> configuration, ILogger<DiscordBotService> logger)
    {
        _client = client;
        _logger = logger;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Ready += HandleReadyAsync;
        _client.Log += HandleLogMessage;
        
        await _client.LoginAsync(TokenType.Bot, _configuration.Token);
        await _client.StartAsync();
    }

    private async Task HandleReadyAsync()
    {
        await _client.SetStatusAsync(UserStatus.Offline);
        await _client.SetGameAsync("cards against humanity");
    }

    private Task HandleLogMessage(LogMessage message)
    {
        if (message.Exception != null)
        {
            _logger.LogCritical("Exception: {exception}", message.Exception);
        }
        
        _logger.LogInformation("{message}", message.Message);

        return Task.CompletedTask;
    }
}