using CardsAgainstDiscord.Data;
using CardsAgainstDiscord.Model;
using CardsAgainstDiscord.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CardsAgainstDiscord.Services;

public class GameService : IGameService
{
    private readonly IDbContextFactory<CardsDbContext> _factory;

    public GameService(IDbContextFactory<CardsDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Game> CreateGameAsync(Lobby lobby)
    {
        await using var context = await _factory.CreateDbContextAsync();

        var game = new Game
        {
            GuildId = lobby.GuildId,
            ChannelId = lobby.ChannelId
        };

        context.Games.Add(game);
        context.Lobbies.Remove(lobby);

        await context.SaveChangesAsync();

        return game;
    }
}