namespace CardsAgainstDiscord.Exceptions;

public class EmbeddableException : ApplicationException
{
    public string Title { get; } 
    
    public string? Description { get; }

    protected EmbeddableException(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }
}