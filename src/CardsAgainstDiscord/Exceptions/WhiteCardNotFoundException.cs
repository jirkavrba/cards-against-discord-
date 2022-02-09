namespace CardsAgainstDiscord.Exceptions;

public class WhiteCardNotFoundException : EmbeddableException
{
    public WhiteCardNotFoundException() : base("Sorry, this white card was not found in the database")
    {
    }
}