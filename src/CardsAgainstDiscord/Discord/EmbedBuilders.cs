using System.Text.Json;
using CardsAgainstDiscord.Extensions;
using Discord;

namespace CardsAgainstDiscord.Discord;

public static class EmbedBuilders
{
    public static Embed Error(string title, string? description = null)
    {
        return new EmbedBuilder()
            .WithColor(DiscordConstants.ColorRed)
            .WithTitle(title)
            .WithDescription(description)
            .WithCurrentTimestamp()
            .Build();
    }

    public static Embed LobbyEmbed(ulong ownerId, IEnumerable<ulong> joinedPlayers, int winPoints)
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
            .WithFooter($"First player to reach {winPoints} points wins")
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
            .AddField("Judge", "⚖️ " + judge.AsUserMention())
            .AddField("Players", string.Join("\n", players))
            .Build();

    public static Embed WhiteCardSelectionEmbed(string blackCard, List<string> pickedCards,
        IEnumerable<string> availableCards)
    {
        var whiteCards = availableCards.Select(card => $" • {card}");

        return new EmbedBuilder()
            .WithTitle("Select a white card to fill in the blank")
            .WithDescription(blackCard.FormatBlackCard(pickedCards))
            .AddField("Available white cards", string.Join("\n", whiteCards))
            .Build();
    }

    public static Embed JudgeSelectionEmbed(ulong judge, IEnumerable<string> submissions) =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorYellow)
            .WithTitle("The judge is choosing this round's winner")
            .WithDescription(string.Join("\n\n", submissions.Select((text, i) => $"`{i + 1}`: {text}")))
            .AddField(" Judge", "⚖️ " + judge.AsUserMention())
            .Build();

    public static Embed RoundWinnerEmbed(string text, ulong winnerId, Dictionary<ulong, int> scoreboard)
    {
        var emojis = new[] {"🥇", "🥈", "🥉"};
        var sorted = scoreboard
            .OrderByDescending(pair => pair.Value)
            .Select((pair, i) =>
            {
                var emoji = i < emojis.Length ? emojis[i] : "🗿";
                var score = pair.Value.ToString().PadLeft(3);
                var user = pair.Key.AsUserMention();

                return $"{emoji} **{score}** - {user}";
            });

        return new EmbedBuilder()
            .WithColor(DiscordConstants.ColorGreen)
            .WithTitle("The judge has selected a winner for this round!")
            .WithDescription(text)
            .WithCurrentTimestamp()
            .AddField("Submitted by", "🏆 " + winnerId.AsUserMention())
            .AddField("Scores", string.Join("\n", sorted))
            .Build();
    }
    
    public static Embed GameWinnerEmbed(ulong winnerUserId, Dictionary<ulong, int> scoreboard)
    {
        var emojis = new[] {"🥇", "🥈", "🥉"};
        var sorted = scoreboard
            .OrderByDescending(pair => pair.Value)
            .Select((pair, i) =>
            {
                var emoji = i < emojis.Length ? emojis[i] : "🗿";
                var score = pair.Value.ToString().PadLeft(3);
                var user = pair.Key.AsUserMention();

                return $"{emoji} **{score}** - {user}";
            });

        return new EmbedBuilder()
            .WithColor(DiscordConstants.ColorYellow)
            .WithTitle($"The game is over!")
            .WithDescription($"{winnerUserId.AsUserMention()} won the game.")
            .WithThumbnailUrl(DiscordConstants.TrophyAnimation)
            .AddField("Scores", string.Join("\n", sorted))
            .WithCurrentTimestamp()
            .Build();
    }

    public static Embed JoinedGameEmbed() =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorGreen)
            .WithThumbnailUrl(DiscordConstants.BannerInactive)
            .WithTitle("You're in!")
            .WithDescription("You will join the game next round")
            .WithCurrentTimestamp()
            .Build();

    public static Embed LeavedGameAsync() =>
        new EmbedBuilder()
            .WithColor(DiscordConstants.ColorGreen)
            .WithTitle("Thanks for stopping by!")
            .WithDescription("You will be removed the game next round")
            .WithCurrentTimestamp()
            .Build();

    public static Embed GameDeletedEmbed() =>
        new EmbedBuilder()
            .WithTitle("Game ended, because there was only 1 player left")
            .WithDescription("Thanks for playing!\nTo create a new game use the `/game` slash command.")
            .WithCurrentTimestamp()
            .Build();
}