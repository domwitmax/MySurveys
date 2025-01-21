using MySurveys.Shared.Models.Questions;

namespace MySurveys.Server.Interfaces.Services;

public interface ISurveyService
{
    public bool CheckSurvey(Survey survey);
    public Task<int?> AddSurvey(Survey survey, string userName);
    public Task<Survey?> GetSurvey(int surveyId);
    public Task<bool> UpdateSurvey(Survey survey, string userName);
    public Task<bool> DeleteSurvey(int surveyId, string userName);
}