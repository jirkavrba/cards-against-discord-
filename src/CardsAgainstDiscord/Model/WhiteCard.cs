using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class WhiteCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }    
    
    /// <summary>
    /// Text of the white card
    /// </summary>
    [Required]
    public string Text { get; set; } = null!;
}