namespace CardsAgainstDiscord.Exceptions;

public class PlayerIsNotJudgeException : EmbeddableException
{
    public PlayerIsNotJudgeException() : base("Only the current judge can do that")
    {
    }
}