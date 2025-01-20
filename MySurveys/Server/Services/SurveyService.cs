using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Questions;
using MySurveys.Shared.Models.Responses;
using System;

namespace MySurveys.Server.Services;

public class SurveyService : ISurveyService
{
    private readonly ISurveyRepository surveyRepository;
    private readonly IUserService userService;
    public SurveyService(ISurveyRepository surveyRepository, IUserService userService)
    {
        this.surveyRepository = surveyRepository;
        this.userService = userService;
    }
    public bool CheckSurvey(Survey survey)
    {
        if(survey.Headers.Length != survey.Options.Length)
            return false;
        if(survey.Headers.Length == 0 ||  survey.Options.Length == 0) 
            return false;
        foreach(var option in  survey.Options)
        {
            switch(option.Type)
            {
                case QuestionEnum.OneChoice:
                case QuestionEnum.MultipleChoice:
                case QuestionEnum.DropDown:
                case QuestionEnum.ListBox:
                case QuestionEnum.SelectOptionsInTable:
                case QuestionEnum.OrganizingItemList:
                case QuestionEnum.SelectIcon:
                    if (option.Choices is null)
                        return false;
                    if(option.Choices.Count() <= 0)
                        return false;
                    break;
                default:
                    if (option.Choices is not null)
                        return false;
                    break;
            };
            if(option.Type == QuestionEnum.Image)
            {
                if(option.Path is null)
                    return false;
                if(option.Path.Length == 0)
                    return false;
                if (!(Uri.TryCreate(option.Path, UriKind.Absolute, out Uri? uriResult) && (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps)))
                    return false;
                if (option.Path.Length == 0) 
                    return false;
                if(option.ImageWidth <= 0)
                    return false;
                if(option.ImageHeight <= 0)
                    return false;
            }
            else
            {
                if (option.Path is not null || option.ImageWidth is not null || option.ImageHeight is not null)
                    return false;
            }
        }
        for(int i=1; i<=survey.Headers.Length; i++)
        {
            if (survey.Headers[i-1].Id != i)
                return false;
            if (survey.Headers[i-1].HttpContent is not null)
            {
                if (string.IsNullOrEmpty(survey.Headers[i-1].HttpContent))
                    return false;
                if (string.IsNullOrWhiteSpace(survey.Headers[i-1].HttpContent))
                    return false;
            }
        }
        return true;
    }
    public async Task<int?> AddSurvey(Survey survey, string userName)
    {
        UserResponse? userResponse = await userService.GetUser(userName);
        if (userResponse is null)
            return null;

        return surveyRepository.AddSurvey(survey, userName);
    }
    public Survey? GetSurvey(int surveyId)
    {
        return surveyRepository.GetSurvey(surveyId);
    }

    public async Task<bool> UpdateSurvey(Survey survey, string userName)
    {
        UserResponse? userResponse = await userService.GetUser(userName);
        if (userResponse is null)
            return false;
        return surveyRepository.UpdateSurvey(survey, userName);
    }

    public async Task<bool> DeleteSurvey(int surveyId, string userName)
    {
        UserResponse? userResponse = await userService.GetUser(userName);
        if (userResponse is null)
            return false;
        return surveyRepository.RemoveSurvey(surveyId, userName);
    }
}
