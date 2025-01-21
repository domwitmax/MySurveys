using Microsoft.EntityFrameworkCore;
using MySurveys.Server.Data;
using MySurveys.Server.Data.Entity;
using MySurveys.Server.Data.Models;
using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Shared.Models.Questions;

namespace MySurveys.Server.Repositores;

public class SurveyRepository : ISurveyRepository
{
    private readonly MySurveysDbContext mySurveysDbContext;
    public SurveyRepository(MySurveysDbContext surveysDbContext)
    {
        mySurveysDbContext = surveysDbContext;
    }
    #region privateQuestionMethods
    private bool updateQuestion(Survey survey, QuestionEntity question, int i)
    {
        if (mySurveysDbContext.Question is null || mySurveysDbContext.HtmlHeaders is null || mySurveysDbContext.OptionImage is null || mySurveysDbContext.OptionChoices is null)
            return false;
        try
        {
            question.QuestionTitle = survey.Headers[i].Title;
            if (question.HtmlId is not null)
            {
                HtmlHeaderEntity? htmlHeaderEntity = mySurveysDbContext.HtmlHeaders.SingleOrDefault(item => item.Id == question.HtmlId);
                if (survey.Headers[i].HttpContent is null && htmlHeaderEntity is not null)
                {
                    mySurveysDbContext.HtmlHeaders.Remove(htmlHeaderEntity);
                    question.HtmlId = null;
                }
                else if (survey.Headers[i].HttpContent is null && htmlHeaderEntity is null)
                {
                    question.HtmlId = null;
                }
                else if (survey.Headers[i].HttpContent is not null && htmlHeaderEntity is not null)
                {
                    htmlHeaderEntity.HtmlContent = survey.Headers[i].HttpContent ?? "";
                    mySurveysDbContext.HtmlHeaders.Update(htmlHeaderEntity);
                }
                else if (survey.Headers[i].HttpContent is not null && htmlHeaderEntity is null)
                {
                    HtmlHeaderEntity htmlHeader = new HtmlHeaderEntity(survey.Headers[i].HttpContent ?? "");
                    mySurveysDbContext.HtmlHeaders.Add(htmlHeader);
                    mySurveysDbContext.SaveChanges();
                    question.HtmlId = htmlHeader.Id;
                }
            }
            else if (question.HtmlId is null && survey.Headers[i].HttpContent is not null)
            {
                HtmlHeaderEntity htmlHeaderEntity = new HtmlHeaderEntity(survey.Headers[i].HttpContent ?? "");
                mySurveysDbContext.HtmlHeaders.Add(htmlHeaderEntity);
                mySurveysDbContext.SaveChanges();
                question.HtmlId = htmlHeaderEntity.Id;
            }
            if (question.ImageId is not null)
            {
                OptionImageEntity? optionImageEntity = mySurveysDbContext.OptionImage.SingleOrDefault(item => item.Id == question.ImageId);
                if (survey.Options[i].Path is null && optionImageEntity is null)
                {
                    question.ImageId = null;
                }
                else if (survey.Options[i].Path is null && optionImageEntity is not null)
                {
                    mySurveysDbContext.OptionImage.Remove(optionImageEntity);
                    mySurveysDbContext.SaveChanges();
                    question.ImageId = null;
                }
                else if (survey.Options[i].Path is not null && optionImageEntity is not null)
                {
                    optionImageEntity.Path = survey.Options[i].Path ?? "";
                    optionImageEntity.Width = survey.Options[i].ImageWidth ?? 0;
                    optionImageEntity.Height = survey.Options[i].ImageHeight ?? 0;
                    mySurveysDbContext.OptionImage.Update(optionImageEntity);
                }
                else if (survey.Options[i].Path is not null && optionImageEntity is null)
                {
                    optionImageEntity = new OptionImageEntity(survey.Options[i].Path ?? "", survey.Options[i].ImageWidth ?? 0, survey.Options[i].ImageHeight ?? 0);
                    mySurveysDbContext.OptionImage.Add(optionImageEntity);
                    mySurveysDbContext.SaveChanges();
                    question.ImageId = optionImageEntity.Id;
                }
            }
            OptionChoicesEntity[] optionChoicesEntities = mySurveysDbContext.OptionChoices.Where(item => item.SurveyId == survey.Id && item.QuestionId == question.QuestionId).ToArray();
            if (optionChoicesEntities.Length == 0 && survey.Options[i].Choices is null)
            {
                mySurveysDbContext.OptionChoices.RemoveRange(optionChoicesEntities);
                mySurveysDbContext.SaveChanges();
            }
            else if (optionChoicesEntities.Length == 0 && survey.Options[i].Choices is not null)
            {
                foreach (string choice in survey.Options[i].Choices ?? new string[] { })
                {
                    OptionChoicesEntity optionChoicesEntity = new OptionChoicesEntity(question.QuestionId, survey.Id, choice);
                    mySurveysDbContext.OptionChoices.Add(optionChoicesEntity);
                }
                mySurveysDbContext.SaveChanges();
            }
            question.Type = survey.Options[i].Type;
            mySurveysDbContext.Question.Update(question);
            return true;
        }
        catch
        {
            return false;
        }
    }
    private bool addQuestion(Survey survey, int i)
    {
        if (mySurveysDbContext.Question is null || mySurveysDbContext.HtmlHeaders is null || mySurveysDbContext.OptionImage is null || mySurveysDbContext.OptionChoices is null)
            return false;
        try
        {
            HtmlHeaderEntity? htmlHeaderEntity = null;
            OptionImageEntity? optionImageEntity = null;

            if (survey.Options[i].Path is not null && survey.Options[i].ImageWidth is not null && survey.Options[i].ImageHeight is not null)
            {
                optionImageEntity = new OptionImageEntity(survey.Options[i].Path ?? "", survey.Options[i].ImageWidth ?? 0, survey.Options[i].ImageHeight ?? 0);
                mySurveysDbContext.OptionImage.Add(optionImageEntity);
            }
            if (survey.Headers[i].HttpContent is not null)
            {
                htmlHeaderEntity = new HtmlHeaderEntity(survey.Headers[i].HttpContent ?? "");
                mySurveysDbContext.HtmlHeaders.Add(htmlHeaderEntity);
            }
            foreach (string option in survey.Options[i].Choices ?? Array.Empty<string>())
            {
                mySurveysDbContext.OptionChoices.Add(new OptionChoicesEntity(i + 1, survey.Id, option));
            }
            mySurveysDbContext.SaveChanges();
            QuestionEntity questionEntity = new QuestionEntity(i + 1, survey.Id, survey.Headers[i].Title, null, null, survey.Options[i].Type);
            if (htmlHeaderEntity is not null)
            {
                questionEntity.HtmlId = htmlHeaderEntity.Id;
            }
            if (optionImageEntity is not null)
            {
                questionEntity.ImageId = optionImageEntity.Id;
            }
            mySurveysDbContext.Question.Add(questionEntity);
            mySurveysDbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
    private bool removeQuestion(int surveyId, int questionId)
    {
        try
        {
            if (mySurveysDbContext.Question is null || mySurveysDbContext.HtmlHeaders is null || mySurveysDbContext.OptionImage is null || mySurveysDbContext.OptionChoices is null)
                return false;
            OptionChoicesEntity[]? optionChoicesEntities = mySurveysDbContext.OptionChoices.Where(item => item.SurveyId == surveyId && item.QuestionId == questionId + 1).ToArray();
            if (optionChoicesEntities is not null)
                mySurveysDbContext.OptionChoices.RemoveRange(optionChoicesEntities);
            QuestionEntity? questionEntity = mySurveysDbContext.Question.SingleOrDefault(item => item.SurveyId == surveyId && item.QuestionId == questionId);
            if (questionEntity is null)
                return false;
            if (questionEntity.HtmlId is not null)
            {
                HtmlHeaderEntity? htmlHeaderEntity = mySurveysDbContext.HtmlHeaders.SingleOrDefault(item => item.Id == questionEntity.HtmlId);
                if (htmlHeaderEntity is not null)
                {
                    mySurveysDbContext.HtmlHeaders.Remove(htmlHeaderEntity);
                }
            }
            if (questionEntity.ImageId is not null)
            {
                OptionImageEntity? optionImageEntity = mySurveysDbContext.OptionImage.SingleOrDefault(item => item.Id == questionEntity.ImageId);
                if (optionImageEntity is not null)
                {
                    mySurveysDbContext.OptionImage.Remove(optionImageEntity);
                }
            }
            mySurveysDbContext.Question.Remove(questionEntity);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
    public async Task<int?> AddSurvey(Survey survey, string userName)
    {
        Dictionary<int,HtmlHeaderEntity> htmlHeader = new Dictionary<int,HtmlHeaderEntity>();
        Dictionary<int,OptionImageEntity> optionImage = new Dictionary<int,OptionImageEntity>();
        SurveyEntity surveyEntity = new SurveyEntity(userName);
        if (mySurveysDbContext.Surveys is not null)
            mySurveysDbContext.Surveys.Add(surveyEntity);
        await mySurveysDbContext.SaveChangesAsync();
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
            await mySurveysDbContext.SaveChangesAsync();
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
            await mySurveysDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return null;
        }
        return surveyEntity.Id;
    }
    public async Task<Survey?> GetSurvey(int surveyId)
    {
        Survey survey = new Survey();
        SurveyEntity? surveyEntity = null;
        if (mySurveysDbContext.Surveys is not null)
            surveyEntity = await mySurveysDbContext.Surveys.SingleOrDefaultAsync(item => item.Id == surveyId);
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
            questionQuestion.Id = question.QuestionId + 1;
            questionQuestion.Title = question.QuestionTitle;
            HtmlHeaderEntity? htmlHeader = await mySurveysDbContext.HtmlHeaders.SingleOrDefaultAsync(item => item.Id == question.HtmlId);
            if (htmlHeader is not null)
            {
                questionQuestion.HttpContent = htmlHeader.HtmlContent;
            }
            else
                questionQuestion.HttpContent = null;
            headerQuestions.Add(questionQuestion);

            OptionQuestion optionQuestion = new OptionQuestion();
            optionQuestion.Type = question.Type;
            OptionImageEntity? optionImage = await mySurveysDbContext.OptionImage.SingleOrDefaultAsync(item => item.Id == question.ImageId);
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
        var questionOptions = mySurveysDbContext.OptionChoices.Where(item => item.SurveyId == surveyId).ToList().GroupBy(item => item.QuestionId).AsEnumerable();
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
    public async Task<bool> UpdateSurvey(Survey survey, string userName)
    {
        Survey? getSurvey = await GetSurvey(survey.Id);
        if(getSurvey is null)
            return false;
        if(mySurveysDbContext.Surveys is null)
            return false;
        SurveyEntity? surveyEntity = await mySurveysDbContext.Surveys.SingleOrDefaultAsync(item => item.UserName == userName);
        if(surveyEntity is null)
            return false;
        if(surveyEntity.Id != survey.Id)
            return false;
        if(mySurveysDbContext.Question is null)
            return false;
        try
        {
            if (survey.Headers.Length == getSurvey.Headers.Length)
            {
                QuestionEntity[] questions = mySurveysDbContext.Question.ToArray();
                for (int i = 0; i < survey.Headers.Length; i++)
                {
                    if(!updateQuestion(survey, questions[i], i))
                        return false;
                }
            }
            else if(survey.Headers.Length > getSurvey.Headers.Length)
            {
                QuestionEntity[] questions = mySurveysDbContext.Question.ToArray();
                int i = 0;
                for(; i < getSurvey.Headers.Length; i++)
                {
                    if(!updateQuestion(survey, questions[i], i))
                        return false;
                }
                for(; i < survey.Headers.Length; i++)
                {
                    if(!addQuestion(survey, i))
                        return false;
                }
            }
            else
            {
                QuestionEntity[] questions = mySurveysDbContext.Question.ToArray();
                int i = 0;
                for (; i < survey.Headers.Length; i++)
                {
                    if(!updateQuestion(survey, questions[i], i))
                        return false;
                }
                for (; i < getSurvey.Headers.Length; i++)
                {
                    if(!removeQuestion(survey.Id, i))
                        return false;
                }
            }
        }
        catch
        {
            return false;
        }
        return true;
    }
    public async Task<bool> RemoveSurvey(int surveyId, string userName)
    {
        if (mySurveysDbContext.Surveys is null || mySurveysDbContext.Question is null || mySurveysDbContext.HtmlHeaders is null || mySurveysDbContext.OptionImage is null || mySurveysDbContext.OptionChoices is null)
            return false;
        try
        {
            SurveyEntity? surveyEntity = await mySurveysDbContext.Surveys.SingleOrDefaultAsync(item => item.Id == surveyId);
            if (surveyEntity is null)
                return false;
            if (surveyEntity.UserName != userName)
                return false;
            QuestionEntity[]? questionEntities = mySurveysDbContext.Question.Where(item => item.SurveyId == surveyId).ToArray();
            if (questionEntities is null)
                return false;
            OptionChoicesEntity[]? optionChoicesEntities = mySurveysDbContext.OptionChoices.Where(item => item.SurveyId == surveyId).ToArray();
            if (optionChoicesEntities is not null)
                mySurveysDbContext.RemoveRange(optionChoicesEntities);
            foreach (QuestionEntity questionEntity in questionEntities)
            {
                if (questionEntity.HtmlId is not null)
                {
                    HtmlHeaderEntity? htmlHeaderEntity = await mySurveysDbContext.HtmlHeaders.SingleOrDefaultAsync(item => item.Id == questionEntity.HtmlId);
                    if (htmlHeaderEntity is not null)
                        mySurveysDbContext.Remove(htmlHeaderEntity);
                }
                if (questionEntity.ImageId is not null)
                {
                    OptionImageEntity? optionImageEntity = await mySurveysDbContext.OptionImage.SingleOrDefaultAsync(item => item.Id == questionEntity.ImageId);
                    if (optionImageEntity is not null)
                        mySurveysDbContext.Remove(optionImageEntity);
                }
            }
            mySurveysDbContext.Question.RemoveRange(questionEntities);
            mySurveysDbContext.Surveys.Remove(surveyEntity);
            await mySurveysDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}