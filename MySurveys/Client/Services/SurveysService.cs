using MySurveys.Client.Interface.Service;
using MySurveys.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace MySurveys.Client.Services;

public class SurveysService : ISurveysService
{
    private IHttpClientFactory httpClientFactory;
    public SurveysService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }
    public async Task<Survey?> GetSurveyAsync(int id, int timeout = 5000)
    {
        Survey? survey = new Survey();
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };
        HttpClient client = httpClientFactory.CreateClient("MySurveys.ServerAPI");
        string path = $"api/survey/{id}";
        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);
            survey = await client.GetFromJsonAsync<Survey>(path, jsonSerializerOptions, cancellationTokenSource.Token);
        }
        catch(OperationCanceledException)
        {
            return null;
        }
        return survey;
    }

    public async Task<bool> SendAnswersAsync(int id, string[] answers)
    {
        SurveyAnswer surveyAnswer = new SurveyAnswer();
        surveyAnswer.Id = id;
        surveyAnswer.Answers = answers;

        JsonSerializerOptions jsonSerializerOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };
        HttpClient client = httpClientFactory.CreateClient("MySurveys.ServerAPI");
        string path = "api/survey";
        try
        {
            HttpResponseMessage result = await client.PostAsJsonAsync<SurveyAnswer>(path, surveyAnswer, jsonSerializerOptions);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
