using Microsoft.EntityFrameworkCore;
using MySurveys.Server.Data;
using MySurveys.Server.Data.Entity;
using MySurveys.Server.Interfaces.Repositores;

namespace MySurveys.Server.Repositores;

public class AnswersRepository : IAnswersRepository
{
    private readonly MySurveysDbContext mySurveysDbContext;
    public AnswersRepository(MySurveysDbContext surveysDbContext)
    {
        mySurveysDbContext = surveysDbContext;
    }
    public async Task<int?> AddAnswers(int surveyId, string answers, string? userName)
    {
        if(surveyId < 1)
            return null;
        if(answers == string.Empty)
            return null;
        AnswerEntity answerEntity = new AnswerEntity(surveyId, answers, userName);
        try
        {
            if (mySurveysDbContext.Answers is null)
                return null;
            mySurveysDbContext.Answers.Add(answerEntity);
            await mySurveysDbContext.SaveChangesAsync();
            return answerEntity.Id;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> GetAnswer(int surveyId, string userName)
    {
        if(mySurveysDbContext.Answers is null)
            return string.Empty;
        AnswerEntity[] answerEntity = await mySurveysDbContext.Answers.Where(item => item.SurveyId == surveyId && item.UserName == userName).ToArrayAsync();
        if(answerEntity.Length == 0 || answerEntity.Length > 1)
            return string.Empty;
        return answerEntity[0].Answers;
    }

    public async Task<string[]> GetAnswers(int surveyId, string userName)
    {
        if (mySurveysDbContext.Answers is null || mySurveysDbContext.Surveys is null)
            return Array.Empty<string>();
        SurveyEntity[] surveyEntity = await mySurveysDbContext.Surveys.Where(item => item.Id == surveyId).ToArrayAsync();
        if (surveyEntity.Length == 0 || surveyEntity.Length > 1)
            return Array.Empty<string>();
        if (surveyEntity[0].UserName != userName)
            return Array.Empty<string>();
        return await mySurveysDbContext.Answers.Where(item => item.SurveyId == surveyId).Select(item => item.Answers).ToArrayAsync();
    }

    public async Task<bool?> RemoveAnswer(int surveyId, string userName)
    {
        if (mySurveysDbContext.Answers is null)
            return null;
        try
        {
            AnswerEntity? answerEntity = await mySurveysDbContext.Answers.SingleOrDefaultAsync(item => item.SurveyId == surveyId && item.UserName == userName);
            if (answerEntity is null)
                return false;
            mySurveysDbContext.Answers.Remove(answerEntity);
            await mySurveysDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<bool?> UpdateAnswer(int surveyId, string userName, string newAnswers)
    {
        if(newAnswers.Length == 0)
            return false;
        try
        {
            if (mySurveysDbContext.Answers is null)
                return false;
            var answerEntity = mySurveysDbContext.Answers.Where(item => item.SurveyId == surveyId && item.UserName == userName).ToArray();
            if(answerEntity.Length == 0 || answerEntity.Length > 1)
                return false;
            answerEntity[0].Answers = newAnswers;
            await mySurveysDbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return null;
        }
    }
}