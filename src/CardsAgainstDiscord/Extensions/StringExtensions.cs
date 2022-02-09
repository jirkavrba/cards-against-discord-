using System.Text.RegularExpressions;

namespace CardsAgainstDiscord.Extensions;

public static class StringExtensions
{
    public static string FormatBlackCard(this string original, List<string>? cards = null)
    {
        var text = original.Replace("\\n", "\n").ReplaceLineEndings();

        var pattern = new Regex("_+");
        const string replacement = "`________`";
        const string replacementAlternative = "`        `";

        // If there are no already selected cards just highlight all blanks and return
        if (cards == null)
        {
            return pattern.Replace(text, replacement);
        }
        
        // If there are no blanks -> eg. question and answer cards, put one on the next line
        if (!pattern.IsMatch(original))
        {
            text = text + " _";
        }

        // Fill-in all card texts (formatted in bold)
        var filled = cards.Aggregate(text, (current, card) => pattern.Replace(current, "**" + card + "**", 1));
        
        // Replace the next blank with the highlighted replacement
        var replaced = pattern.Replace(filled, replacement);
            
        // Replace all other blanks with the alternative replacement
        return pattern.Replace(replaced, replacementAlternative);
    }

    public static string SafeSubstring(this string original, int start, int length)
    {
        return start >= original.Length
            ? string.Empty
            : original.Substring(start, Math.Min(start + length, original.Length - start));
    }
}