using MySurveys.Server.Data;
using MySurveys.Server.Data.Entity;
using MySurveys.Server.Data.Models;
using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Shared.Models.Questions;
using System.Collections.Immutable;

namespace MySurveys.Server.Repositores;

public class SurveyRepository : ISurveyRepository
{
    private readonly MySurveysDbContext mySurveysDbContext;
    public SurveyRepository(MySurveysDbContext surveysDbContext)
    {
        mySurveysDbContext = surveysDbContext;
    }
    public int? AddSurvey(Survey survey, string userName)
    {
        Dictionary<int,HtmlHeaderEntity> htmlHeader = new Dictionary<int,HtmlHeaderEntity>();
        Dictionary<int,OptionImageEntity> optionImage = new Dictionary<int,OptionImageEntity>();
        SurveyEntity surveyEntity = new SurveyEntity(userName);
        if (mySurveysDbContext.Surveys is not null)
            mySurveysDbContext.Surveys.Add(surveyEntity);
        mySurveysDbContext.SaveChanges();
        try
        {
            for (int i = 0; i < survey.Headers.Length; i++)
            {
                if (survey.Headers[i].HttpContent is not null)
                {
                    HtmlHeaderEntity tmp = new HtmlHeaderEntity(survey.Headers[i].HttpContent ?? "");
                    htmlHeader.Add(i, tmp);
                    if (mySurveysDbContext.HtmlHeaders is not null)
                        mySurveysDbContext.HtmlHeaders.Add(tmp);
                }
                if (survey.Options[i].Path is not null)
                {
                    OptionImageEntity tmp = new OptionImageEntity(survey.Options[i].Path ?? "", survey.Options[i].ImageWidth ?? 0, survey.Options[i].ImageHeight ?? 0);
                    optionImage.Add(i, tmp);
                    if (mySurveysDbContext.OptionImage is not null)
                        mySurveysDbContext.OptionImage.Add(tmp);
                }
                if (survey.Options[i].Choices is not null)
                {
                    foreach (string choice in survey.Options[i].Choices ?? new List<string>())
                    {
                        OptionChoicesEntity tmp = new OptionChoicesEntity(i, surveyEntity.Id, choice);
                        if (mySurveysDbContext.OptionChoices is not null)
                            mySurveysDbContext.OptionChoices.Add(tmp);
                    }
                }
            }
            mySurveysDbContext.SaveChanges();
            for (int i = 0; i < survey.Headers.Length; i++)
            {
                int? htmlId = null;
                if (htmlHeader.ContainsKey(i))
                    htmlId = htmlHeader[i].Id;
                int? imageId = null;
                if (optionImage.ContainsKey(i))
                    imageId = optionImage[i].Id;
                QuestionEntity tmp = new QuestionEntity(i, surveyEntity.Id, survey.Headers[i].Title, htmlId, imageId, survey.Options[i].Type);
                if (mySurveysDbContext.Question is not null)
                    mySurveysDbContext.Question.Add(tmp);
            }
            mySurveysDbContext.SaveChanges();
        }
        catch (Exception)
        {
            return null;
        }
        return surveyEntity.Id;
    }

    public Survey? GetSurvey(int surveyId)
    {
        Survey survey = new Survey();

        SurveyEntity? surveyEntity = null;
        if (mySurveysDbContext.Surveys is not null)
            surveyEntity = mySurveysDbContext.Surveys.SingleOrDefault(item => item.Id == surveyId);
        if (surveyEntity is null)
            return null;
        survey.Id = surveyEntity.Id;

        IEnumerable<QuestionEntity>? questions = null;
        if (mySurveysDbContext.Question is not null)
            questions = mySurveysDbContext.Question.Where(item => item.SurveyId == surveyId).OrderBy(item => item.QuestionId);
        if (questions is null)
            return null;

        if (mySurveysDbContext.HtmlHeaders is null || mySurveysDbContext.OptionImage is null)
            return null;
        List<HeaderQuestion> headerQuestions = new List<HeaderQuestion>();
        List<OptionQuestion> optionQuestions = new List<OptionQuestion>();
        foreach(QuestionEntity question in questions.ToArray())
        {
            HeaderQuestion questionQuestion = new HeaderQuestion();
            questionQuestion.Id = question.Id;
            questionQuestion.Title = question.QuestionTitle;
            HtmlHeaderEntity? htmlHeader = mySurveysDbContext.HtmlHeaders.SingleOrDefault(item => item.Id == question.HtmlId);
            if (htmlHeader is not null)
            {
                questionQuestion.HttpContent = htmlHeader.HtmlContent;
            }
            else
                questionQuestion.HttpContent = null;
            headerQuestions.Add(questionQuestion);

            OptionQuestion optionQuestion = new OptionQuestion();
            optionQuestion.Type = question.Type;
            OptionImageEntity? optionImage = mySurveysDbContext.OptionImage.SingleOrDefault(item => item.Id == question.ImageId);
            if(optionImage is not null)
            {
                optionQuestion.ImageWidth = optionImage.Width;
                optionQuestion.ImageHeight = optionImage.Height;
                optionQuestion.Path = optionImage.Path;
            }
            optionQuestions.Add(optionQuestion);
        }
        if (mySurveysDbContext.OptionChoices is null)
            return null;
        var questionOptions = mySurveysDbContext.OptionChoices.Where(item => item.SurveyId == surveyId).GroupBy(item => item.QuestionId);
        foreach (var group in questionOptions)
        {
            int? i = group.First().QuestionId;
            if (i is null)
                break;
            IEnumerable<string> optionChoices = group.OrderBy(item => item.Id).ToList().Select(item => item.Option);
            optionQuestions[i ?? 0].Choices = optionChoices;
        }
        survey.Headers = headerQuestions.ToArray();
        survey.Options = optionQuestions.ToArray();
        return survey;
    }

    public bool RemoveSurvey(Guid surveyId)
    {
        throw new NotImplementedException();
    }

    public bool UpdateSurvey(Survey survey)
    {
        throw new NotImplementedException();
    }
}