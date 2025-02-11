namespace MySurveys.Server.Interfaces.Repositores;

public interface IAnswersRepository
{
    public Task<int?> AddAnswers(int surveyId, string answers, string? userName);
    public Task<string> GetAnswer(int surveyId, string userName);
    public Task<string[]> GetAnswers(int surveyId, string userName);
    public Task<bool?> UpdateAnswer(int surveyId, string userName, string newAnswers);
    public Task<bool?> RemoveAnswer(int surveyId, string userName);
}