namespace MySurveys.Shared;

public class HeaderQuestion
{
    public readonly string Title;
    public readonly int Id;
    public readonly string? HttpContent;
    public HeaderQuestion(string title, int id)
    {
        Title = title;
        Id = id;
        HttpContent = null;
    }
    public HeaderQuestion(string title, int id, string httpContent)
    {
        Title = title; 
        Id = id; 
        HttpContent = httpContent;
    }
}
