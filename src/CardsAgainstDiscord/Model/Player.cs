using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Player
{
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
    ///     Id of the currently selected white card (so that there can be confirmation button)
    /// </summary>
    public int? SelectedWhiteCardId { get; set; } = null;

    /// <summary>
    ///     Mapped game that this player belongs to
    /// </summary>
    public Game Game { get; set; } = null!;

    /// <summary>
    ///     Mapped selected white card
    /// </summary>
    public WhiteCard? SelectedWhiteCard { get; set; } = null;

    /// <summary>
    ///     Mapped games where the player is the judge
    /// </summary>
    public IEnumerable<Game> JudgedGames { get; set; } = new List<Game>();

    /// <summary>
    ///     Mapped list of picked white cards (in all rounds)
    /// </summary>
    public IEnumerable<PickedCard> PickedCards { get; set; } = new List<PickedCard>();

    /// <summary>
    ///     Mapped white cards that this player has in his hand
    /// </summary>
    public IEnumerable<WhiteCard> WhiteCards { get; set; } = new List<WhiteCard>();
}