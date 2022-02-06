using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 
    
    /// <summary>
    /// ID of the Discord user that this player entity is associated to
    /// </summary>
    [Required]
    public ulong UserId { get; set; }

    /// <summary>
    /// Mapped white cards that this player has in his hand
    /// </summary>
    public List<WhiteCard> WhiteCards = new();
}