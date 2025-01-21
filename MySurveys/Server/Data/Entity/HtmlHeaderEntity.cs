namespace MySurveys.Server.Data.Entity;

public class HtmlHeaderEntity
{
    public int Id { get; set; }
    public string HtmlContent { get; set; }
    public HtmlHeaderEntity(string htmlContent)
    {
        HtmlContent = htmlContent;
    }
}