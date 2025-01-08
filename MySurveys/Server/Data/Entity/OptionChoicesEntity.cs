namespace MySurveys.Server.Data.Entity;

public class OptionChoicesEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public int SurveyId { get; set; }
    public string Option {  get; set; }
    public OptionChoicesEntity(int questionId, int surveyId, string option)
    {
        QuestionId = questionId;
        SurveyId = surveyId;
        Option = option;
    }
}