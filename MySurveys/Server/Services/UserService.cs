using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MySurveys.Server.Services;

public class UserService : IUserService
{
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;
    public UserService(SignInManager<IdentityUser> signInManager, IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<bool> ChangePassword(ChangePasswordUser changePasswordUser)
    {
        IdentityUser? user = await userManager.FindByNameAsync(changePasswordUser.UserName);
        if (user is null)
        {
            return false;
        }
        IdentityResult result = await userManager.ChangePasswordAsync(user,changePasswordUser.OldPassword,changePasswordUser.NewPassword);
        if(result.Succeeded)
            return true;
        return false;
    }

    public async Task<DeleteUserResponse> DeleteUser(DeleteUser deleteUser)
    {
        IdentityUser? user = await userManager.FindByNameAsync(deleteUser.UserName);
        if (user is null)
        {
            return new DeleteUserResponse() { Success = false };
        }
        IdentityResult result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            //TODO: Add remove all deleted user sureys and user data 
            return new DeleteUserResponse() { Success = true };
        }
        return new DeleteUserResponse() { Success = false };
    }

    public async Task<UserResponse?> GetUser(string username)
    {
        IdentityUser? user = await userManager.FindByNameAsync(username);
        if (user is null)
            return null;
        UserResponse response = new UserResponse();
        response.UserId = user.Id;
        response.UserName = username;
        return response;
    }

    public async Task<LoginResponse> Login(LoginUser login)
    {
        IdentityUser? user = await userManager.FindByNameAsync(login.UserName);
        if (user == null)
        {
            return new LoginResponse() { Success = false, TokenJwt = null };
        }
        var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
        if (result.Succeeded)
        {
            return new LoginResponse()
            {
                Success = true,
                TokenJwt = null
            };
        }
        else
            return new LoginResponse() { Success = false, TokenJwt=null };
    }
    public async Task<RegisterResponse> Register(RegisterUser register)
    {
        IdentityUser newUser = new IdentityUser() { UserName = register.UserName };
        IdentityResult result = await userManager.CreateAsync(newUser, register.Password);
        RegisterResponse response = new RegisterResponse();
        if(result.Succeeded)
        {
            response.Success = true;
            IdentityUser? user = await userManager.FindByNameAsync(register.UserName);
            if(user == null)
            {
                response.Success = false;
                response.TokenJwt = null;
            }
            else
                response.TokenJwt = await GenerateJwtToken(user);
        }
        else
        {
            response.Success = false;
            response.TokenJwt = null;
        }
        return response;
    }

    public async Task<UpdateUserResponse> UpdateUser(UpdateUser updateUser)
    {
        IdentityUser? user = await userManager.FindByNameAsync(updateUser.UserName ?? string.Empty);
        if (user == null)
        {
            return new UpdateUserResponse() { Success = false, UserName = null };
        }
        //TODO: add updated data to user
        IdentityResult result = await userManager.UpdateAsync(user);
        if(result.Succeeded)
        {
            return new UpdateUserResponse() { Success = true, UserName = updateUser.UserName };
        }
        return new UpdateUserResponse() { Success = false, UserName = null };
    }
    private async Task<string> GenerateJwtToken(IdentityUser user)
    {
        if (user.UserName is null || configuration["Jwt:Key"] is null)
            return string.Empty;
        var userClaims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        }
        .Union(userClaims)
        .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
