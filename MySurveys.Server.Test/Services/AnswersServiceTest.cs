using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySurveys.Server.Data;
using MySurveys.Server.Data.Entity;
using MySurveys.Server.Interfaces.Repositores;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Server.Repositores;
using MySurveys.Server.Services;
using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;

namespace MySurveys.Server.Test.Services;

public class AnswersServiceTest
{
    private ServiceProvider? getServiceProvider()
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
        services.AddScoped<IAnswersRepository, AnswersRepository>();
        services.AddScoped<IAnswersService, AnswerService>();

        IEnumerable<KeyValuePair<string, string?>> inMemorySettings = new List<KeyValuePair<string, string?>>()
        {
            new KeyValuePair<string, string?>("Jwt:Key", "f6c78dcd933913c2d20c3fc324a021a62b697d88a31c93d11baf0411f357cfa5"),
            new KeyValuePair<string, string?>("Jwt:Issuer", "MySurvey")
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        return services.BuildServiceProvider();
    }
    #region AddAnswer
    [Fact]
    public async Task TestAddAnswerCorrectWithoutUserName()
    {
        // Arrange
        IAnswersService? answersService = getServiceProvider()?.GetService<IAnswersService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = null;

        // Act
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;

        // Assert
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
    }
    [Fact]
    public async Task TestAddAnswerCorrectWithUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
    }
    [Fact]
    public async Task TestAddAnswerWrongId()
    {
        // Arrange
        IAnswersService? answersService = getServiceProvider()?.GetService<IAnswersService>();
        int surveyId = -1;
        string answer = "2;3;2:3:4";
        string? userName = null;

        // Act
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;

        // Assert

        Assert.NotNull(answersService);
        Assert.Null(answerId);
    }
    [Fact]
    public async Task TestAddAnswerWrongAnswer()
    {
        // Arrange
        IAnswersService? answersService = getServiceProvider()?.GetService<IAnswersService>();
        int surveyId = 1;
        string answer = "";
        string? userName = null;

        // Act
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;

        // Assert
        Assert.NotNull(answersService);
        Assert.Null(answerId);
    }
    [Fact]
    public async Task TestAddAnswerWrongUserName()
    {
        // Arrange
        IAnswersService? answersService = getServiceProvider()?.GetService<IAnswersService>();
        int surveyId = -1;
        string answer = "2;3;2:3:4";
        string? userName = "test";

        // Act
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;

        // Assert
        Assert.NotNull(answersService);
        Assert.Null(answerId);
    }
    #endregion
    #region GetAnswer
    [Fact]
    public async Task TestGetAnswerCorrectUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        string? result = answersService is not null ? await answersService.GetAnswer(surveyId, userName) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(result);
        Assert.NotEqual(string.Empty, result);
    }
    [Fact]
    public async Task TestGetAnswerWrongUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string wrongUserName = "wrongUserName";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        string? result = answersService is not null ? await answersService.GetAnswer(surveyId, wrongUserName) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.Equal(string.Empty, result);
    }
    #endregion
    #region GetAnswers
    [Fact]
    public async Task TestGetAnswersCorrectUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        MySurveysDbContext? mySurveysDbContext = serviceProvider?.GetService<MySurveysDbContext>();
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);
        SurveyEntity surveyEntity = new SurveyEntity(userName);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        if(mySurveysDbContext is not null && mySurveysDbContext.Surveys is not null)
        {
            mySurveysDbContext.Surveys.Add(surveyEntity);
            mySurveysDbContext.SaveChanges();
        }
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyEntity.Id, answer, userName) : null;
        string[]? result = answersService is not null ? await answersService.GetAnswers(surveyEntity.Id, userName) : null;

        // Assert
        Assert.NotNull(mySurveysDbContext);
        Assert.NotNull(mySurveysDbContext.Answers);
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
    [Fact]
    public async Task TestGetAnswersWrongUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        MySurveysDbContext? mySurveysDbContext = serviceProvider?.GetService<MySurveysDbContext>();
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string wrongUserName = "wrongUserName";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);
        SurveyEntity surveyEntity = new SurveyEntity(userName);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        if (mySurveysDbContext is not null && mySurveysDbContext.Surveys is not null)
        {
            mySurveysDbContext.Surveys.Add(surveyEntity);
            mySurveysDbContext.SaveChanges();
        }
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyEntity.Id, answer, userName) : null;
        string[]? result = answersService is not null ? await answersService.GetAnswers(surveyEntity.Id, wrongUserName) : null;

        // Assert
        Assert.NotNull(mySurveysDbContext);
        Assert.NotNull(mySurveysDbContext.Answers);
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    #endregion
    #region UpdateAnswer
    [Fact]
    public async Task TestUpdateAnswerCorrect()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string newAnswer = "2;3";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        bool? isUpdated = answersService is not null ? await answersService.UpdateAnswer(surveyId, userName, newAnswer) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(isUpdated);
        Assert.True(isUpdated);
    }
    [Fact]
    public async Task TestUpdateAnswerWrongUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string newAnswer = "2;3";
        string? userName = "test";
        string wrongUserName = "wrongUserName";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        bool? isUpdated = answersService is not null ? await answersService.UpdateAnswer(surveyId, wrongUserName, newAnswer) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(isUpdated);
        Assert.False(isUpdated);
    }
    [Fact]
    public async Task TestUpdateAnswerEmptyNewAnswer()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string newAnswer = "";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        bool? isUpdated = answersService is not null ? await answersService.UpdateAnswer(surveyId, userName, newAnswer) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(isUpdated);
        Assert.False(isUpdated);
    }
    #endregion
    #region RemoveAnswer
    [Fact]
    public async Task TestRemoveAnswerCorrect()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        bool? isDeleted = answersService is not null ? await answersService.RemoveAnswer(surveyId, userName) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(isDeleted);
        Assert.True(isDeleted);
    }
    [Fact]
    public async Task TestRemoveAnswerWrongUserName()
    {
        // Arrange
        var serviceProvider = getServiceProvider();
        IAnswersService? answersService = serviceProvider?.GetService<IAnswersService>();
        IUserService? userService = serviceProvider?.GetService<IUserService>();
        int surveyId = 1;
        string answer = "2;3;2:3:4";
        string? userName = "test";
        string wrongUserName = "wrongUserName";
        string password = "zaq1@WSX";
        RegisterUser registerUser = new RegisterUser(userName, password);

        // Act
        RegisterResponse? registerResponse = userService is not null ? await userService.Register(registerUser) : null;
        int? answerId = answersService is not null ? await answersService.AddAnswers(surveyId, answer, userName) : null;
        bool? isDeleted = answersService is not null ? await answersService.RemoveAnswer(surveyId, wrongUserName) : null;

        // Assert
        Assert.NotNull(registerResponse);
        Assert.NotNull(answersService);
        Assert.NotNull(answerId);
        Assert.True(answerId > 0);
        Assert.NotNull(isDeleted);
        Assert.False(isDeleted);
    }
    #endregion
}