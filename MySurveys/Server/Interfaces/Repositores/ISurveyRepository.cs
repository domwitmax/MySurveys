using MySurveys.Shared.Models.Questions;

namespace MySurveys.Server.Interfaces.Repositores;

public interface ISurveyRepository
{
    public int? AddSurvey(Survey survey, string userName);
    public Survey? GetSurvey(int surveyId);
    public bool UpdateSurvey(Survey survey, string userName);
    public bool RemoveSurvey(int surveyId, string userName);
}