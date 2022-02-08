using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class GameRound
{
   /// <summary>
   ///     Cards picked in this round by all players
   /// </summary>
   public List<PickedCard> PickedCards = new();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     ID of the message embed for this round
    /// </summary>
    public ulong MessageId { get; set; }

    /// <summary>
    ///     ID of the game that this round belongs to
    /// </summary>
    [Required]
    public int GameId { get; set; }

    /// <summary>
    ///     ID of the black card that was selected for this round
    /// </summary>
    [Required]
    public int BlackCardId { get; set; }

    /// <summary>
    ///     ID of the player that was selected as the judge
    /// </summary>
    [Required]
    public int JudgeId { get; set; }

    /// <summary>
    ///     Game that this round belongs to
    /// </summary>
    public Game Game { get; set; } = null!;

    /// <summary>
    ///     Black card that was selected for this round
    /// </summary>
    public BlackCard BlackCard { get; set; } = null!;

    /// <summary>
    ///     Player chosen as the judge in this round
    /// </summary>
    public Player Judge { get; set; } = null!;
}