using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class BlackCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Text of the black card
    /// </summary>
    [Required]
    public string Text { get; set; } = null!;

    /// <summary>
    /// Number of white cards required to complete this black card
    /// </summary>
    [Required]
    public int Picks { get; set; } = 0;
}