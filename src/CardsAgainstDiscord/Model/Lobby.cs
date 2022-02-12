using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Lobby
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     ID of the guild that the lobby was created in
    /// </summary>
    [Required]
    public ulong GuildId { get; init; }

    /// <summary>
    ///     ID of the channel that the lobby was created in
    /// </summary>
    [Required]
    public ulong ChannelId { get; init; }

    /// <summary>
    ///     ID of the message with the lobby embed
    /// </summary>
    [Required]
    public ulong MessageId { get; init; }

    /// <summary>
    ///     Discord ID of the owner that created this lobby
    /// </summary>
    [Required]
    public ulong OwnerId { get; init; }

    /// <summary>
    ///     Number of points required in order to win the game
    /// </summary>
    [Required]
    public int WinPoints { get; init; }
    
    /// <summary>
    ///     IDs of the players joined to this lobby
    /// </summary>
    [Required]
    public List<ulong> JoinedPlayers { get; init; } = new();
}