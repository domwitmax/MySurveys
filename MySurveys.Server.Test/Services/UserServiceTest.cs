using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySurveys.Server.Services;
using MySurveys.Shared.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MySurveys.Server.Data;
using System.Text;

namespace MySurveys.Server.Test.Services;

public class UserServiceTest
{
    private UserService getService()
    {
        var services = new ServiceCollection();

        services.AddDbContext<MySurveysDbContext>(options =>
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString()));

        services.AddLogging();

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<MySurveysDbContext>()
            .AddDefaultTokenProviders();

        var serviceProvider = services.BuildServiceProvider();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "f6c78dcd933913c2d20c3fc324a021a62b697d88a31c93d11baf0411f357cfa5"},
            {"Jwt:Issuer", "MySurvey"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        return new UserService(configuration, userManager);
    }
    private ClaimsPrincipal? validateToken(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("f6c78dcd933913c2d20c3fc324a021a62b697d88a31c93d11baf0411f357cfa5");

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidIssuer = "MySurvey",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
    #region Register
    [Fact]
    public async Task TestRegisterHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string100";
        string password = "string2@W";
        var model = new RegisterUser(name, password);

        // Act
        var result = await service.Register(model);
        var claims = validateToken(result.TokenJwt);
        string? userName = claims?.FindFirstValue(ClaimTypes.Name);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(userName);
        Assert.Equal(name, userName);
    }
    [Fact]
    public async Task TestRegisterWrongPassword()
    {
        // Arrange
        UserService service = getService();
        string name = "string101";
        string password = "string";
        var model = new RegisterUser(name, password);

        // Act
        var result = await service.Register(model);
        var claims = validateToken(result.TokenJwt);

        // Assert
        Assert.False(result.Success);
        Assert.Null(claims);
    }
    [Fact]
    public async Task TestRegisterWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string102";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new RegisterUser(name, password);

        // Act
        var result = await service.Register(model);
        var claims = validateToken(result.TokenJwt);

        // Assert
        Assert.False(result.Success);
        Assert.Null(claims);
    }
    #endregion
    #region Login
    [Fact]
    public async Task TestLoginHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string103";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new LoginUser(name, password);

        // Act
        var result = await service.Login(model);
        var claims = validateToken(result.TokenJwt);
        string? userName = claims?.FindFirstValue(ClaimTypes.Name);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(claims);
        Assert.NotNull(userName);
        Assert.Equal(name, userName);
    }
    [Fact]
    public async Task TestLoginWrongPassword()
    {
        // Arrange
        UserService service = getService();
        string name = "string104";
        string password = "string2@W";
        string wrongPassword = "string";
        await service.Register(new RegisterUser(name, password));
        var model = new LoginUser(name, wrongPassword);

        // Act
        var result = await service.Login(model);
        var claims = validateToken(result.TokenJwt);

        // Assert
        Assert.False(result.Success);
        Assert.Null(claims);
    }
    [Fact]
    public async Task TestLoginWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string105";
        string wrongName = "string";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new LoginUser(wrongName, password);

        // Act
        var result = await service.Login(model);
        var claims = validateToken(result.TokenJwt);

        // Assert
        Assert.False(result.Success);
        Assert.Null(claims);
    }
    #endregion
    #region GetUser
    [Fact]
    public async Task TestGetUserHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string106";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = name;

        // Act
        var result = await service.GetUser(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(model, result.UserName);
    }
    [Fact]
    public async Task TestGetUserWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string107";
        string wrongName = "string";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = name;

        // Act
        var result = await service.GetUser(wrongName);

        // Assert
        Assert.Null(result);
        Assert.NotEqual(model, result?.UserName);
    }
    #endregion
    #region UpdateUser
    [Fact]
    public async Task TestUpdateUserHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string108";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new UpdateUser() { UserName = name };

        // Act
        var result = await service.UpdateUser(model);

        // Assert
        Assert.Equal(name, result.UserName);
    }
    [Fact]
    public async Task TestUpdateUserWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string109";
        string wrongName = "string";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new UpdateUser() { UserName = wrongName };

        // Act
        var result = await service.UpdateUser(model);

        // Assert
        Assert.NotEqual(name, result?.UserName);
    }
    #endregion
    #region ChangePasswordUser
    [Fact]
    public async Task TestChangePasswordHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string110";
        string password = "string2@W";
        string newPassword = "string3#E";
        await service.Register(new RegisterUser(name, password));
        var model = new ChangePasswordUser() { UserName = name, OldPassword = password, NewPassword = newPassword };

        // Act
        var result = await service.ChangePassword(model);
        var loginResult = await service.Login(new LoginUser(name, newPassword));
        var claims = validateToken(loginResult.TokenJwt);
        string? userName = claims?.FindFirstValue(ClaimTypes.Name);

        // Assert
        Assert.True(result);
        Assert.True(loginResult.Success);
        Assert.NotNull(claims);
        Assert.NotNull(userName);
        Assert.Equal(name, userName);
    }
    [Fact]
    public async Task TestChangePasswordWrongPassword()
    {
        // Arrange
        UserService service = getService();
        string name = "string111";
        string password = "string2@W";
        string newPassword = "string";
        await service.Register(new RegisterUser(name, password));
        var model = new ChangePasswordUser() { UserName = name, OldPassword = password, NewPassword = newPassword };

        // Act
        var result = await service.ChangePassword(model);
        var loginResult = await service.Login(new LoginUser(name, newPassword));
        var claims = validateToken(loginResult.TokenJwt);
        string? userName = claims?.FindFirstValue(ClaimTypes.Name);

        // Assert
        Assert.False(result);
        Assert.NotNull(loginResult);
        Assert.False(loginResult.Success);
        Assert.Null(claims);
        Assert.Null(userName);
        Assert.NotEqual(name, userName);
    }
    [Fact]
    public async Task TestChangePasswordWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string112";
        string wrongName = "string";
        string password = "string2@W";
        string newPassword = "string3#E";
        await service.Register(new RegisterUser(name, password));
        var model = new ChangePasswordUser() { UserName = wrongName, OldPassword = password, NewPassword = newPassword };

        // Act
        var result = await service.ChangePassword(model);

        // Assert
        Assert.False(result);
    }
    #endregion
    #region DeleteUser
    [Fact]
    public async Task TestDeleteUserHappyPath()
    {
        // Arrange
        UserService service = getService();
        string name = "string113";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new DeleteUser() { UserName = name };

        // Act
        var result = await service.DeleteUser(model);

        // Assert
        Assert.True(result.Success);
    }
    [Fact]
    public async Task TestDeleteUserWrongLogin()
    {
        // Arrange
        UserService service = getService();
        string name = "string114";
        string wrongName = "string";
        string password = "string2@W";
        await service.Register(new RegisterUser(name, password));
        var model = new DeleteUser() { UserName = wrongName };

        // Act
        var result = await service.DeleteUser(model);

        // Assert
        Assert.False(result.Success);
    }
    #endregion
}