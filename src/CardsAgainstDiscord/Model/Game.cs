using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// ID of the current game round
    /// </summary>
    public int? CurrentRoundId { get; set; } = null;

    /// <summary>
    /// ID of the guild that this game is being played in
    /// </summary>
    [Required]
    public ulong GuildId { get; set; }

    /// <summary>
    /// ID of the channel that this game is being played in
    /// </summary>
    [Required]
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Currently played game round
    /// </summary>
    public GameRound? CurrentRound { get; set; } = null;
}