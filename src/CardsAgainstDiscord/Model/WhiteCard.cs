using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class WhiteCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     Text of the white card
    /// </summary>
    [Required]
    public string Text { get; set; } = null!;

    /// <summary>
    ///     Mapped list of all picks this white card was selected in
    /// </summary>
    public List<PickedCard> Picks { get; set; } = new();

    /// <summary>
    ///     Mapped list of players that have this white card in their hands
    /// </summary>
    public List<Player> Players { get; set; } = new();

    /// <summary>
    ///     Mapped list of games that this card was used in
    /// </summary>
    public List<Game> Games { get; set; } = new();
}