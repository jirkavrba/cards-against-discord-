using System.Text;

namespace CardsAgainstDiscord.Extensions;

public static class StringExtensions
{
    public static string ToBlackCardText(this string original)
    {
        return original.Replace("_", "`          `");
    }
}