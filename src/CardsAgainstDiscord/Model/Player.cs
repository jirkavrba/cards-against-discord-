using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Player
{
    /// <summary>
    ///     Mapped list of all rounds where this player is the judge
    /// </summary>
    public List<GameRound> JudgingRounds = new();

    /// <summary>
    ///     Mapped list of picked white cards (in all rounds)
    /// </summary>
    public List<PickedCard> PickedCards = new();

    /// <summary>
    ///     Mapped white cards that this player has in his hand
    /// </summary>
    public List<WhiteCard> WhiteCards = new();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     ID of the Discord user that this player entity is associated to
    /// </summary>
    [Required]
    public ulong UserId { get; set; }

    /// <summary>
    ///     ID of the game that this player belongs to
    /// </summary>
    [Required]
    public int GameId { get; set; }

    /// <summary>
    ///     Mapped game that this player belongs to
    /// </summary>
    public Game Game { get; set; } = null!;
}