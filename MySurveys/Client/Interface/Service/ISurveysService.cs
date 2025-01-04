using MySurveys.Shared;

namespace MySurveys.Client.Interface.Service;

public interface ISurveysService
{
    Task<Survey?> GetSurveyAsync(int id, int timeout = 5000);
    Task<bool> SendAnswersAsync(int id, string[] answers);
}
