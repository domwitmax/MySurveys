namespace MySurveys.Server.Data.Entity;

public class SurveyEntity
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public SurveyEntity(string userName)
    {
        UserName = userName;
    }
}
