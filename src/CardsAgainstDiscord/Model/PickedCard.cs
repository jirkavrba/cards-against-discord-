using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class PickedCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     ID of the player that picked this card
    /// </summary>
    [Required]
    public int PlayerId { get; set; }

    /// <summary>
    ///     ID of the white card that was picked
    /// </summary>
    [Required]
    public int WhiteCardId { get; set; }

    /// <summary>
    ///     Mapped player that made this pick
    /// </summary>
    public Player Player { get; set; } = null!;

    /// <summary>
    ///     Mapped white card that was picked
    /// </summary>
    public WhiteCard WhiteCard { get; set; } = null!;
}