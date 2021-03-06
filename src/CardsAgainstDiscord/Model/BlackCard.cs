using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CardsAgainstDiscord.Extensions;

namespace CardsAgainstDiscord.Model;

public class BlackCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     Text of the black card
    /// </summary>
    [Required]
    public string Text { get; set; } = null!;
    
    /// <summary>
    ///     Text formatted for better display in Discord embeds
    /// </summary>
    public string FormattedText => Text.FormatBlackCard();

    /// <summary>
    ///     Number of white cards required to complete this black card
    /// </summary>
    [Required]
    public int Picks { get; set; } = 0;

    /// <summary>
    ///     List of all games that this card was picked in
    /// </summary>
    public List<Game> Games { get; set; } = null!;
}