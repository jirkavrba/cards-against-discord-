using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class WhiteCard
{
    /// <summary>
    ///     Mapped list of all picks this white card was selected in
    /// </summary>
    public List<PickedCard> Picks = new();

    /// <summary>
    ///     Mapped list of players that have this white card in their hands
    /// </summary>
    public List<Player> Players = new();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     Text of the white card
    /// </summary>
    [Required]
    public string Text { get; set; } = null!;
}