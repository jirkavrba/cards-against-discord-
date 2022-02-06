using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

[Table("white_cards")]
public class WhiteCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }    
    
    /// <summary>
    /// Text of the white card
    /// </summary>
    [Required]
    [Column("text")]
    public string Text { get; set; } = null!;
}