using CardsAgainstDiscord.Model;

namespace CardsAgainstDiscord.Services.Contracts;

public interface IGamesService
{
    public Task<Game> CreateGameAsync(Lobby lobby);

    public Task<BlackCard?> GetCurrentBlackCardAsync(int gameId);

    public Task<List<WhiteCard>> GetAvailableWhiteCardsAsync(int gameId, ulong userId);
    
    public Task<List<WhiteCard>> GetPickedWhiteCardsAsync(int gameId, ulong userId);

    public Task SelectWhiteCardAsync(int gameId, ulong userId, int whiteCardId);
    
    public Task<bool> ConfirmSelectedCardAsync(int gameId, ulong userId);

    public Task<(Player, string)> SubmitWinnerAsync(int gameId, ulong playerId, int winnerId);
}