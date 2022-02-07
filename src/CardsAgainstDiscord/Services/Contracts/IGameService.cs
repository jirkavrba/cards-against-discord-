using CardsAgainstDiscord.Model;

namespace CardsAgainstDiscord.Services.Contracts;

public interface IGameService
{
    public Task<Game> CreateGameAsync(Lobby lobby);
    
}