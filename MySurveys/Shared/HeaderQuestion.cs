namespace MySurveys.Shared;

public class HeaderQuestion
{
    public string Title { get; set; }
    public int Id { get; set; }
    public string? HttpContent { get; set; }
    public HeaderQuestion()
    {

    }
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
