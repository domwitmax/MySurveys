namespace MySurveys.Server.Data.Entity;

public class AnswerEntity
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string? UserName { get; set; }
    public string Answers { get; set; }
    public AnswerEntity(int surveyId, string answers, string? userName)
    {
        SurveyId = surveyId;
        UserName = userName;
        Answers = answers;
    }
}