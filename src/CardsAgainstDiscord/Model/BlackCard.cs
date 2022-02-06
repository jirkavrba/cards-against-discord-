using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

[Table("black_cards")]
public class BlackCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Text of the black card
    /// </summary>
    [Required]
    [Column("text")]
    public string Text { get; set; } = null!;

    /// <summary>
    /// Number of white cards required to complete this black card
    /// </summary>
    [Required]
    [Column("picks")]
    public int Picks { get; set; } = 0;
}