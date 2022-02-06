namespace CardsAgainstDiscord.Configuration;

public class DiscordConfiguration
{
    public const string Section = "Discord";
        
    public DiscordConfiguration(string token)
    {
        Token = token;
    }

    /// <summary>
    /// Discord token used for gateway authentication
    /// </summary>
    public string Token { get; }
}