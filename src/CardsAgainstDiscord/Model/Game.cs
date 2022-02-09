using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardsAgainstDiscord.Model;

public class Game
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     ID of the guild that this game is being played in
    /// </summary>
    [Required]
    public ulong GuildId { get; set; }

    /// <summary>
    ///     ID of the channel that this game is being played in
    /// </summary>
    [Required]
    public ulong ChannelId { get; set; }

    /// <summary>
    ///     ID of the message with the current round
    /// </summary>
    public ulong? MessageId { get; set; } = null;

    /// <summary>
    ///     ID of the currently selected black card
    /// </summary>
    public int? BlackCardId { get; set; } = null;

    /// <summary>
    ///     ID of the currently judging player
    /// </summary>
    public int? JudgeId { get; set; } = null;

    /// <summary>
    ///     ID of the winner selected by the judging player
    /// </summary>
    public int? SelectedWinnerId { get; set; } = null;

    /// <summary>
    ///     Mapped black card selected for this round
    /// </summary>
    public BlackCard? BlackCard { get; set; } = null;

    /// <summary>
    ///     Mapped player that is the current Card czar (judge)
    /// </summary>
    public Player? Judge { get; set; } = null;

    /// <summary>
    ///     Mapped list of players that belong to this game
    /// </summary>
    public List<Player> Players { get; set; } = new();

    /// <summary>
    ///     List of players that want to join the game before next round
    /// </summary>
    public List<ulong> JoiningPlayers { get; set; } = new();

    /// <summary>
    ///     List of players that want to leave the game before next round
    /// </summary>
    public List<ulong> LeavingPlayers { get; set; } = new();

    public List<WhiteCard> UsedWhiteCards { get; set; } = new();

    public List<BlackCard> UsedBlackCards { get; set; } = new();
}