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
}