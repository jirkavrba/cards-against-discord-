using System.Text.Json;
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

    public static Embed LobbyEmbed(ulong ownerId, IEnumerable<ulong> joinedPlayers)
    {
        var ids = joinedPlayers.Select(p => p.AsUserMention()).ToList();
        var players = ids.Any() ? string.Join(", ", ids) : "_No player have joined this game yet_";
        
        return new EmbedBuilder()
            .WithColor(DiscordConstants.ColorPrimary)
            .WithThumbnailUrl(DiscordConstants.Banner)
            .WithTitle("Let's play cards against humanity!")
            .WithDescription("To join or leave this game, click the button below the message.\nYou can leave the game by clicking the button again.")
            .AddField("Game owner", ownerId.AsUserMention())
            .AddField("Joined players", players)
            .WithCurrentTimestamp()
            .Build();
    }

    public static Embed CancelledLobbyEmbed() =>
        new EmbedBuilder()
            .WithThumbnailUrl(DiscordConstants.BannerInactive)
            .WithTitle("Game cancelled by the owner.")
            .WithDescription("To create a new game, please use the `/game` command")
            .WithCurrentTimestamp()
            .Build();

    public static Embed GameRoundEmbed(string blackCard, ulong judge, IEnumerable<string> players) =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorPrimary)
            .WithTitle(blackCard)
            .WithDescription("To pick your card, click the button below this message.")
            .AddField("Judge", judge.AsUserMention())
            .AddField("Players", string.Join("\n", players))
            .Build();

    public static Embed WhiteCardSelectionEmbed(string blackCard, List<string> pickedCards, IEnumerable<string> availableCards)
    {
        var whiteCards = availableCards.Select(card => $" • {card}");

        return new EmbedBuilder()
            .WithTitle("Select a white card to fill in the blank")
            .WithDescription(blackCard.FormatBlackCard(pickedCards))
            .AddField("Available white cards", string.Join("\n", whiteCards))
            .Build();
    }

    public static Embed AllWhiteCardsPickedEmbed() =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorGreen)
            .WithTitle("All white cards picked!")
            .WithDescription("Now wait for other players to finish.\nYou can dismiss all previous emphemeral message.")
            .Build();
}