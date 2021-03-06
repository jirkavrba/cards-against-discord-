using CardsAgainstDiscord.Configuration;
using CardsAgainstDiscord.Discord.Commands;
using CardsAgainstDiscord.Discord.Interactions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace CardsAgainstDiscord.Discord;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;

    private readonly IEnumerable<IComponentHandler> _componentHandlers;

    private readonly IEnumerable<IModalHandler> _modalHandlers;

    private readonly DiscordConfiguration _configuration;

    private readonly ISlashCommandDispatcher _dispatcher;

    private readonly ILogger<DiscordBotService> _logger;

    public DiscordBotService(
        DiscordSocketClient client,
        IOptions<DiscordConfiguration> configuration,
        ILogger<DiscordBotService> logger,
        ISlashCommandDispatcher dispatcher,
        IEnumerable<IComponentHandler> componentHandlers, 
        IEnumerable<IModalHandler> modalHandlers)
    {
        _client = client;
        _logger = logger;
        _dispatcher = dispatcher;
        _componentHandlers = componentHandlers;
        _modalHandlers = modalHandlers;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _client.Ready += HandleReadyAsync;
        _client.Log += HandleLogMessage;
        _client.ButtonExecuted += HandleInteraction;
        _client.SelectMenuExecuted += HandleInteraction;
        _client.ModalSubmitted += HandleModal;

        await _client.LoginAsync(TokenType.Bot, _configuration.Token);
        await _client.StartAsync();
        await Task.Delay(-1, stoppingToken);
    }

    private async Task HandleInteraction(SocketMessageComponent component)
    {
        foreach (var handler in _componentHandlers) await handler.HandleInteractionAsync(component);
    }

    private async Task HandleModal(SocketModal modal)
    {
        foreach (var handler in _modalHandlers) await handler.HandleModalSubmissionAsync(modal);
    }

    private async Task HandleReadyAsync()
    {
        await _dispatcher.RegisterCommandsAsync(_client);

        await _client.SetStatusAsync(UserStatus.Offline);
        await _client.SetGameAsync("cards against humanity");
    }

    private Task HandleLogMessage(LogMessage message)
    {
        if (message.Exception != null) _logger.LogCritical("Exception: {exception}", message.Exception);

        _logger.LogInformation("{message}", message.Message);

        return Task.CompletedTask;
    }
}