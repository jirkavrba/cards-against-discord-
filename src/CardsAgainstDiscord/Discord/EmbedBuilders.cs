using CardsAgainstDiscord.Extensions;
using Discord;

namespace CardsAgainstDiscord.Discord;

public static class EmbedBuilders
{
    public static EmbedBuilder Error(string title, string? description = null)
    {
        return new EmbedBuilder()
            .WithColor(DiscordConstants.ColorRed)
            .WithTitle(title)
            .WithDescription(description)
            .WithCurrentTimestamp();
    }

    public static Embed LobbyEmbed(ulong ownerId, IEnumerable<ulong> joinedPlayers) =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorPrimary)
            .WithThumbnailUrl(DiscordConstants.Banner)
            .WithTitle("Let's play cards against humanity!")
            .WithDescription("To join or leave the game, use the button below.")
            .AddField("Game owner", ownerId.AsUserMention())
            .AddField("Joined players", string.Join(", ", joinedPlayers.Select(p => p.AsUserMention())))
            .WithCurrentTimestamp()
            .Build();
}