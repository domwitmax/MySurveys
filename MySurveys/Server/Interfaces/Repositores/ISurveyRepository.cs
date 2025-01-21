using MySurveys.Shared.Models.Questions;

namespace MySurveys.Server.Interfaces.Repositores;

public interface ISurveyRepository
{
    public Task<int?> AddSurvey(Survey survey, string userName);
    public Task<Survey?> GetSurvey(int surveyId);
    public Task<bool> UpdateSurvey(Survey survey, string userName);
    public Task<bool> RemoveSurvey(int surveyId, string userName);
}