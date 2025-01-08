using MySurveys.Shared.Models.Questions;

namespace MySurveys.Server.Data.Models;

public class QuestionEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public int SurveyId { get; set; }
    public string QuestionTitle { get; set; }
    public int? HtmlId { get; set; }
    public int? ImageId { get; set; }
    public QuestionEnum Type { get; set; }
    public QuestionEntity(int questionId, int surveyId, string questionTitle, int? htmlId, int? imageId, QuestionEnum type)
    {
        QuestionId = questionId;
        SurveyId = surveyId;
        QuestionTitle = questionTitle;
        HtmlId = htmlId;
        ImageId = imageId;
        Type = type;
    }
}