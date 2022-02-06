using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

[Table("lobbies")]
public class Lobby
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }   
    
    /// <summary>
    /// ID of the guild that the lobby was created in
    /// </summary>
    [Column("guild_id")]
    [Required]
    public ulong GuildId { get; set; }
    
    /// <summary>
    /// ID of the channel that the lobby was created in
    /// </summary>
    [Column("channel_id")]
    [Required]
    public ulong ChannelId { get; set; }
    
    /// <summary>
    /// ID of the message with the lobby embed
    /// </summary>
    [Column("message_id")]
    [Required]
    public ulong MessageId { get; set; }
    
    /// <summary>
    /// Id of the owner that created this lobby
    /// </summary>
    [Column("owner_id")]
    [Required]
    public ulong OwnerId { get; set; }
    
    /// <summary>
    /// IDs of the players joined to this lobby 
    /// </summary>
    [Column("joined_players")]
    [Required]
    public List<ulong> JoinedPlayers { get; set; } = new();
}