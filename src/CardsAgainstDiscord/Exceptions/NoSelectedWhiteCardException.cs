namespace CardsAgainstDiscord.Exceptions;

public class NoSelectedWhiteCardException : EmbeddableException
{
    public NoSelectedWhiteCardException() : base(
        "You have not selected any white card",
        "Please use the selection menu below the message"
    )
    {
    }
}