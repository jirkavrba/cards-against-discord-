using CardsAgainstDiscord.Model;

namespace CardsAgainstDiscord.Services.Contracts;

public interface IGamesService
{
    public Task<Game> CreateGameAsync(Lobby lobby);
    
}