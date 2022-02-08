using System.Text.RegularExpressions;

namespace CardsAgainstDiscord.Extensions;

public static class StringExtensions
{
    public static string FormatBlackCard(this string original, List<string>? cards = null)
    {
        var text = original.Replace("\\n", "\n").ReplaceLineEndings();
        var pattern = new Regex("_+");

        const string replacement = " `________` ";

        // If there are no fill-ins just replace all blanks
        if (cards == null) return pattern.Replace(text, replacement);

        // Fill in all already picked cards
        var filled = cards.Aggregate(text, (current, card) => pattern.Replace(current, $"**{card}**", 1));

        // Highlight the first next blank 
        return pattern.Replace(filled, replacement, 1);
    }

    public static string SafeSubstring(this string original, int start, int length)
    {
        return start >= original.Length
            ? string.Empty
            : original.Substring(start, Math.Min(start + length, original.Length - start));
    }
}