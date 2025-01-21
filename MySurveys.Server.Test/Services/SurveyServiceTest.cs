using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySurveys.Server.Data;
using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Server.Repositores;
using MySurveys.Server.Services;
using MySurveys.Shared.Models.Questions;
using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;

namespace MySurveys.Server.Test.Services;

public class SurveyServiceTest
{
    private (ISurveyService?, IUserService?) getServices(bool giveBothService = true)
    {
        var services = new ServiceCollection();

        services.AddDbContext<MySurveysDbContext>(options =>
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));

        services.AddLogging();

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<MySurveysDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISurveyRepository, SurveyRepository>();
        services.AddScoped<ISurveyService, SurveyService>();

        IEnumerable<KeyValuePair<string, string?>> inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("Jwt:Key", "f6c78dcd933913c2d20c3fc324a021a62b697d88a31c93d11baf0411f357cfa5"),
            new KeyValuePair<string, string?>("Jwt:Issuer", "MySurvey")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        var serviceProvider = services.BuildServiceProvider();

        ISurveyService? surveyService = serviceProvider.GetService<ISurveyService>();
        IUserService? userService = serviceProvider.GetService<IUserService>();
        if(giveBothService)
            return (surveyService, userService);
        return (surveyService, null);
    }
    private bool isEqual(Survey? a, Survey? b)
    {
        if(a is null || b is null)
            return false;
        if (a.Headers.Length != b.Headers.Length)
            return false;
        for(int i=0; i<a.Headers.Length; i++)
        {
            if (a.Headers[i].Id != b.Headers[i].Id || a.Headers[i].Title != b.Headers[i].Title || a.Headers[i].HttpContent != b.Headers[i].HttpContent)
                return false;
            if (a.Options[i].Type != b.Options[i].Type) 
                return false;
            if (a.Options[i].Path != b.Options[i].Path || a.Options[i].ImageWidth != b.Options[i].ImageWidth || a.Options[i].ImageHeight != b.Options[i].ImageHeight)
                return false;
            if (a.Options[i].Choices is not null && b.Options[i].Choices is null)
                return false;
            if (a.Options[i].Choices is null && b.Options[i].Choices is not null)
                return false;
            if (a.Options[i].Choices?.Count() != b.Options[i].Choices?.Count())
                return false;
            for(int j=0; j < a.Options[i]?.Choices?.Count(); j++)
            {
                if (a.Options[i]?.Choices?.ElementAt(j) != b.Options[i]?.Choices?.ElementAt(j))
                    return false;
            }
        }
        return true;
    }
    private Survey getCorrectSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>"""),
            new HeaderQuestion("Pytanie wielowierszowe",2),
            new HeaderQuestion("Pytanie jednego wyboru",3),
            new HeaderQuestion("Pytanie wielokrotnego wyboru",4),
            new HeaderQuestion("Pytanie z formatowaniem tekstu",5),
            new HeaderQuestion("Pytanie z listy rozwijanej",6),
            new HeaderQuestion("Pytanie z listy",7),
            new HeaderQuestion("Pytanie z gwiazdkami",8),
            new HeaderQuestion("Pytanie w wyborze z tabeli",9),
            new HeaderQuestion("Pytanie z kliknięciem obrazka",10),
            new HeaderQuestion("Pytanie z układaniem opcji",11),
            new HeaderQuestion("Pytanie z oceną w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
        };
        IEnumerable<string> choices = new string[] { "wybór 1", "wybór 2", "wybór 3", "wybór 4" };
        IEnumerable<string> icons = new string[] { "Sentiment_Satisfied", "Mood", "Sentiment_Very_Satisfied", "Mood_Bad" };
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines),
            new OptionQuestion(QuestionEnum.OneChoice, choices),
            new OptionQuestion(QuestionEnum.MultipleChoice, choices),
            new OptionQuestion(QuestionEnum.HTMLEditor),
            new OptionQuestion(QuestionEnum.DropDown, choices),
            new OptionQuestion(QuestionEnum.ListBox, choices),
            new OptionQuestion(QuestionEnum.Stars),
            new OptionQuestion(QuestionEnum.SelectOptionsInTable, choices),
            new OptionQuestion(QuestionEnum.Image, path, 600, 300),
            new OptionQuestion(QuestionEnum.OrganizingItemList, choices),
            new OptionQuestion(QuestionEnum.SelectRatingScale10),
            new OptionQuestion(QuestionEnum.SelectIcon, icons)

        };
        return survey;
    }
    #region WrongSurvey
    private Survey getNotEqualSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>""")
        };
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines)
        };
        return survey;
    }
    private Survey getEmptyHeaderSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[] { };
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines)
        };
        return survey;
    }
    private Survey getEmptyOptionsSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>""")
        };
        survey.Options = new OptionQuestion[] { };
        return survey;
    }
    private Survey getEmptyHeaderAndOptionsSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[] { };
        survey.Options = new OptionQuestion[] { };
        return survey;
    }
    private Survey getWrongOrderHeaderSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>"""),
            new HeaderQuestion("Pytanie wielowierszowe",2),
            new HeaderQuestion("Pytanie jednego wyboru",6),
            new HeaderQuestion("Pytanie wielokrotnego wyboru",4),
            new HeaderQuestion("Pytanie z formatowaniem tekstu",5),
            new HeaderQuestion("Pytanie z listy rozwijanej",8),
            new HeaderQuestion("Pytanie z listy",7),
            new HeaderQuestion("Pytanie z gwiazdkami",8),
            new HeaderQuestion("Pytanie w wyborze z tabeli",20),
            new HeaderQuestion("Pytanie z kliknięciem obrazka",10),
            new HeaderQuestion("Pytanie z układaniem opcji",11),
            new HeaderQuestion("Pytanie z oceną w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
        };
        IEnumerable<string> choices = new string[] { "wybór 1", "wybór 2", "wybór 3", "wybór 4" };
        IEnumerable<string> icons = new string[] { "Sentiment_Satisfied", "Mood", "Sentiment_Very_Satisfied", "Mood_Bad" };
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines),
            new OptionQuestion(QuestionEnum.OneChoice, choices),
            new OptionQuestion(QuestionEnum.MultipleChoice, choices),
            new OptionQuestion(QuestionEnum.HTMLEditor),
            new OptionQuestion(QuestionEnum.DropDown, choices),
            new OptionQuestion(QuestionEnum.ListBox, choices),
            new OptionQuestion(QuestionEnum.Stars),
            new OptionQuestion(QuestionEnum.SelectOptionsInTable, choices),
            new OptionQuestion(QuestionEnum.Image, path, 600, 300),
            new OptionQuestion(QuestionEnum.OrganizingItemList, choices),
            new OptionQuestion(QuestionEnum.SelectRatingScale10),
            new OptionQuestion(QuestionEnum.SelectIcon, icons)

        };
        return survey;
    }
    private Survey getEmptyHtmlContentSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1,"")
        };
        IEnumerable<string> choices = new string[] { };
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine)
        };
        return survey;
    }
    private Survey getWhiteSpacesHtmlContentSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1,"  ")
        };
        IEnumerable<string> choices = new string[] { };
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine)
        };
        return survey;
    }
    private Survey getEmptyOrNullChoicesSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>"""),
            new HeaderQuestion("Pytanie wielowierszowe",2),
            new HeaderQuestion("Pytanie jednego wyboru",3),
            new HeaderQuestion("Pytanie wielokrotnego wyboru",4),
            new HeaderQuestion("Pytanie z formatowaniem tekstu",5),
            new HeaderQuestion("Pytanie z listy rozwijanej",6),
            new HeaderQuestion("Pytanie z listy",7),
            new HeaderQuestion("Pytanie z gwiazdkami",8),
            new HeaderQuestion("Pytanie w wyborze z tabeli",9),
            new HeaderQuestion("Pytanie z kliknięciem obrazka",10),
            new HeaderQuestion("Pytanie z układaniem opcji",11),
            new HeaderQuestion("Pytanie z oceną w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
        };
        IEnumerable<string> choices = new string[] {};
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines),
            new OptionQuestion(QuestionEnum.OneChoice, null),
            new OptionQuestion(QuestionEnum.MultipleChoice, choices),
            new OptionQuestion(QuestionEnum.HTMLEditor),
            new OptionQuestion(QuestionEnum.DropDown, null),
            new OptionQuestion(QuestionEnum.ListBox, choices),
            new OptionQuestion(QuestionEnum.Stars),
            new OptionQuestion(QuestionEnum.SelectOptionsInTable, choices),
            new OptionQuestion(QuestionEnum.Image, path, 600, 300),
            new OptionQuestion(QuestionEnum.OrganizingItemList, choices),
            new OptionQuestion(QuestionEnum.SelectRatingScale10),
            new OptionQuestion(QuestionEnum.SelectIcon, null)

        };
        return survey;
    }
    private Survey getAdditionalChoicesSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie jednowierszowe",1,"""<h1 style="text-align: center;">Test</h1><h2 style="text-align: center;">Test</h2>"""),
            new HeaderQuestion("Pytanie wielowierszowe",2),
            new HeaderQuestion("Pytanie jednego wyboru",3),
            new HeaderQuestion("Pytanie wielokrotnego wyboru",4),
            new HeaderQuestion("Pytanie z formatowaniem tekstu",5),
            new HeaderQuestion("Pytanie z listy rozwijanej",6),
            new HeaderQuestion("Pytanie z listy",7),
            new HeaderQuestion("Pytanie z gwiazdkami",8),
            new HeaderQuestion("Pytanie w wyborze z tabeli",9),
            new HeaderQuestion("Pytanie z kliknięciem obrazka",10),
            new HeaderQuestion("Pytanie z układaniem opcji",11),
            new HeaderQuestion("Pytanie z oceną w skali 1-10",12),
            new HeaderQuestion("Pytanie z wyborem ikony",13)
        };
        IEnumerable<string> choices = new string[] { "wybór 1", "wybór 2", "wybór 3", "wybór 4" };
        IEnumerable<string> icons = new string[] { "Sentiment_Satisfied", "Mood", "Sentiment_Very_Satisfied", "Mood_Bad" };
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine),
            new OptionQuestion(QuestionEnum.MultipleLines),
            new OptionQuestion(QuestionEnum.OneChoice, choices),
            new OptionQuestion(QuestionEnum.MultipleChoice, choices),
            new OptionQuestion(QuestionEnum.HTMLEditor),
            new OptionQuestion(QuestionEnum.DropDown, choices),
            new OptionQuestion(QuestionEnum.ListBox, choices),
            new OptionQuestion(QuestionEnum.Stars, choices),
            new OptionQuestion(QuestionEnum.SelectOptionsInTable, choices),
            new OptionQuestion(QuestionEnum.Image, path, 600, 300),
            new OptionQuestion(QuestionEnum.OrganizingItemList, choices),
            new OptionQuestion(QuestionEnum.SelectRatingScale10),
            new OptionQuestion(QuestionEnum.SelectIcon, icons)

        };
        return survey;
    }
    private Survey getEmptyPathSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1)
        };
        IEnumerable<string> choices = new string[] { };
        string path = @"";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.Image, path, 600, 300)
        };
        return survey;
    }
    private Survey getWrongPathSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1)
        };
        IEnumerable<string> choices = new string[] { };
        string path = @"httpx://www.radzen.err/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.Image, path, 600, 300)
        };
        return survey;
    }
    private Survey getWrongSizeSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1)
        };
        IEnumerable<string> choices = new string[] { };
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.Image, path, -20, 0)
        };
        return survey;
    }
    private Survey getAdditionalPathAndSizeSurvey()
    {
        Survey survey = new Survey();
        survey.Headers = new HeaderQuestion[]
        {
            new HeaderQuestion("Pytanie z kliknięciem obrazka",1)
        };
        IEnumerable<string> choices = new string[] { };
        string path = @"https://www.radzen.com/assets/radzen-logo-top-b2d6e9dcacf7d344bbab515b8748c5f4d702c6c5bfc349bd9ff9003016a3a6ee.svg";
        survey.Options = new OptionQuestion[]
        {
            new OptionQuestion(QuestionEnum.OneLine, path, 600, 300)
        };
        return survey;
    }
    #endregion
    #region ChceckSurvey
    [Fact]
    public void TestCheckSurveyCorrect()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getCorrectSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.True(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyNotEqual()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getNotEqualSurvey();
        
        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);
        
        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyEmptyHeader()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyHeaderSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyEmptyOptions()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyOptionsSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyEmptyHeaderAndOptions()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyHeaderAndOptionsSurvey();
        
        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyWrongOrderHeader()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getWrongOrderHeaderSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyEmptyHtmlContentHeader()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyHtmlContentSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestCheckSurveyWhiteSpacesHtmlContentHeader()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getWhiteSpacesHtmlContentSurvey();
        
        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);
        
        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestEmptyOrNullChoices()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyOrNullChoicesSurvey();
        
        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestAdditionalChoices()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getAdditionalChoicesSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestEmptyPath()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getEmptyPathSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestWrongPath()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getWrongPathSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestWrongSize()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getWrongSizeSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    [Fact]
    public void TestAdditionalPathAndSizeSurvey()
    {
        // Arrange
        ISurveyService? surveyService = getServices(false).Item1;
        Survey survey = getAdditionalPathAndSizeSurvey();

        // Act
        bool? isCorrect = surveyService?.CheckSurvey(survey);

        // Assert
        Assert.NotNull(isCorrect);
        Assert.False(isCorrect);
    }
    #endregion
    #region AddSurvey
    [Fact]
    public async Task TestAddSurveryCorrectUserName()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "test2@W");
        Survey model = getCorrectSurvey();

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? result = surveyService is not null ? await surveyService.AddSurvey(model, userName) : null;
        
        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(result);
        Assert.True(result > 0);
    }
    [Fact]
    public async Task TestAddSurveryWrongUserName()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        string wrongUserName = "wrongUserName";
        RegisterUser registerUser = new RegisterUser(userName, "test2@W");
        Survey model = getCorrectSurvey();

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? result = surveyService is not null ? await surveyService.AddSurvey(model, wrongUserName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.Null(result);
        Assert.False(result > 0);
    }
    #endregion
    [Fact]
    public async Task TestGetSurvey()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        Survey testSurvey = getCorrectSurvey();
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(testSurvey, userName) : null;
        Survey? result = surveyService is not null ? await surveyService.GetSurvey(surveyId ?? 0) : null;
        bool isEqualSurvey = isEqual(testSurvey, result);
        
        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(result);
        Assert.True(isEqualSurvey);
    }
    [Fact]
    #region UpdateSurvey
    public async Task TestUpdateSurveyCorrectUserNameLessHeaders()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");
        Survey primalSurvey = getCorrectSurvey();
        Survey newSurvey = getCorrectSurvey();
        newSurvey.Headers = newSurvey.Headers.Take(newSurvey.Headers.Length - 5).ToArray();
        newSurvey.Options = newSurvey.Options.Take(newSurvey.Options.Length - 5).ToArray();

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(primalSurvey, userName) : null;
        newSurvey.Id = surveyId is not null ? surveyId.Value : 0;
        bool? isSuccessUpdate = surveyService is not null ? await surveyService.UpdateSurvey(newSurvey, userName) : null;
        
        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessUpdate);
        Assert.True(isSuccessUpdate);
    }
    [Fact]
    public async Task TestUpdateSurveyCorrectUserNameEqualHeaders()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");
        Survey primalSurvey = getCorrectSurvey();
        Survey newSurvey = getCorrectSurvey();
        int questionId = newSurvey.Headers[0].Id;
        newSurvey.Headers[0] = newSurvey.Headers[newSurvey.Headers.Length - 5];
        newSurvey.Headers[0].Id = questionId;
        newSurvey.Options[0] = newSurvey.Options[newSurvey.Options.Length - 5];
        questionId = newSurvey.Headers[2].Id;
        newSurvey.Headers[2] = newSurvey.Headers[newSurvey.Headers.Length - 3];
        newSurvey.Headers[2].Id = questionId;
        newSurvey.Options[2] = newSurvey.Options[newSurvey.Options.Length - 3];
        questionId = newSurvey.Headers[4].Id;
        newSurvey.Headers[4] = newSurvey.Headers[newSurvey.Headers.Length - 1];
        newSurvey.Headers[4].Id = questionId;
        newSurvey.Options[4] = newSurvey.Options[newSurvey.Options.Length - 1];

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(primalSurvey, userName) : null;
        newSurvey.Id = surveyId is not null ? surveyId.Value : 0;
        bool? isSuccessUpdate = surveyService is not null ? await surveyService.UpdateSurvey(newSurvey, userName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessUpdate);
        Assert.True(isSuccessUpdate);
    }
    [Fact]
    public async Task TestUpdateSurveyCorrectUserNameMoreHeaders()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");
        Survey primalSurvey = getCorrectSurvey();
        Survey newSurvey = getCorrectSurvey();
        List<HeaderQuestion> headers = new List<HeaderQuestion>();
        List<OptionQuestion> options = new List<OptionQuestion>();
        headers.AddRange(newSurvey.Headers);
        options.AddRange(newSurvey.Options);
        headers.Add(newSurvey.Headers[2]);
        options.Add(newSurvey.Options[2]);
        headers.Add(newSurvey.Headers[4]);
        options.Add(newSurvey.Options[4]);
        headers.Add(newSurvey.Headers[6]);
        options.Add(newSurvey.Options[6]);
        headers.Add(newSurvey.Headers[8]);
        options.Add(newSurvey.Options[8]);
        newSurvey.Headers = headers.ToArray();
        newSurvey.Options = options.ToArray();
        newSurvey.Headers[newSurvey.Headers.Length - 4].Id = newSurvey.Headers.Length - 4;
        newSurvey.Headers[newSurvey.Headers.Length - 3].Id = newSurvey.Headers.Length - 3;
        newSurvey.Headers[newSurvey.Headers.Length - 2].Id = newSurvey.Headers.Length - 2;
        newSurvey.Headers[newSurvey.Headers.Length - 1].Id = newSurvey.Headers.Length - 1;

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(primalSurvey, userName) : null;
        newSurvey.Id = surveyId is not null ? surveyId.Value : 0;
        bool? isSuccessUpdate = surveyService is not null ? await surveyService.UpdateSurvey(newSurvey, userName) : null;
        
        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessUpdate);
        Assert.True(isSuccessUpdate);
    }
    [Fact]
    public async Task TestUpdateSurveyWrongUserName()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        string wrongUserName = "wrongUserName";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");
        Survey primalSurvey = getCorrectSurvey();
        Survey newSurvey = getCorrectSurvey();
        newSurvey.Headers = newSurvey.Headers.Take(newSurvey.Headers.Length - 5).ToArray();
        newSurvey.Options = newSurvey.Options.Take(newSurvey.Headers.Length - 5).ToArray();

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(primalSurvey, userName) : null;
        newSurvey.Id = surveyId is not null ? surveyId.Value : 0;
        bool? isSuccessUpdate = surveyService is not null ? await surveyService.UpdateSurvey(newSurvey, wrongUserName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessUpdate);
        Assert.False(isSuccessUpdate);
    }
    [Fact]
    public async Task TestUpdateSurveyWrongId()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");
        Survey primalSurvey = getCorrectSurvey();
        Survey newSurvey = getCorrectSurvey();
        newSurvey.Headers = newSurvey.Headers.Take(newSurvey.Headers.Length - 5).ToArray();
        newSurvey.Options = newSurvey.Options.Take(newSurvey.Headers.Length - 5).ToArray();

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(primalSurvey, userName) : null;
        bool? isSuccessUpdate = surveyService is not null ? await surveyService.UpdateSurvey(newSurvey, userName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessUpdate);
        Assert.False(isSuccessUpdate);
    }
    [Fact]
    #endregion
    #region DeleteSurvey
    public async Task TestDeleteSurveyCorrectUserName()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        Survey testSurvey = getCorrectSurvey();
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(testSurvey, userName) : null;
        bool? isSuccessDeletedSurvey = surveyService is not null ? await surveyService.DeleteSurvey(surveyId.Value, userName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessDeletedSurvey);
        Assert.True(isSuccessDeletedSurvey);
    }
    [Fact]
    public async Task TestDeleteSurveyWrongUserName()
    {
        // Arrange
        var services = getServices();
        ISurveyService? surveyService = services.Item1;
        IUserService? userService = services.Item2;
        string userName = "test";
        string wrongUserName = "wrongUserName";
        Survey testSurvey = getCorrectSurvey();
        RegisterUser registerUser = new RegisterUser(userName, "string2@W");

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? surveyId = surveyService is not null ? await surveyService.AddSurvey(testSurvey, userName) : null;
        bool? isSuccessDeletedSurvey = surveyService is not null ? await surveyService.DeleteSurvey(surveyId ?? 0, wrongUserName) : null;

        // Assert
        Assert.NotNull(surveyService);
        Assert.NotNull(userService);
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.Success);
        Assert.NotNull(surveyId);
        Assert.NotNull(isSuccessDeletedSurvey);
        Assert.False(isSuccessDeletedSurvey);
    }
    #endregion
}