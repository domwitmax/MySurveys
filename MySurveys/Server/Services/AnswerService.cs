using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Responses;

namespace MySurveys.Server.Services;

public class AnswerService : IAnswersService
{
    private readonly IAnswersRepository answersRepository;
    private readonly IUserService userService;
    public AnswerService(IAnswersRepository answersRepository, IUserService userService)
    {
        this.answersRepository = answersRepository;
        this.userService = userService;
    }
    public async Task<int?> AddAnswers(int surveyId, string answers, string? userName)
    {
        if(userName is not null)
        {
            UserResponse? userResponse = await userService.GetUser(userName);
            if (userResponse is null)
                return null;
        }
        return await answersRepository.AddAnswers(surveyId, answers, userName);
    }

    public Task<string> GetAnswer(int surveyId, string userName)
    {
        return answersRepository.GetAnswer(surveyId, userName);
    }

    public Task<string[]> GetAnswers(int surveyId, string userName)
    {
        return answersRepository.GetAnswers(surveyId, userName);
    }

    public Task<bool?> UpdateAnswer(int surveyId, string userName, string newAnswers)
    {
        return answersRepository.UpdateAnswer(surveyId, userName, newAnswers);
    }

    public Task<bool?> RemoveAnswer(int surveyId, string userName)
    {
        return answersRepository.RemoveAnswer(surveyId, userName);
    }
}
