namespace CardsAgainstDiscord.Exceptions;

public class NoSelectedSubmissionException : EmbeddableException
{
    public NoSelectedSubmissionException() : base("You have to select a submission first using the menu selection.")
    {
    }
}