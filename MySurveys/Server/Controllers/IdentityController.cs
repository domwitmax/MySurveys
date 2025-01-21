using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;
using System.Security.Claims;

namespace MySurveys.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly IUserService userService;
    public IdentityController(IUserService userService)
    {
        this.userService = userService;
    }
    [HttpPost("Register")]
    [ProducesResponseType<RegisterResponse>(200)]
    [ProducesResponseType<RegisterResponse>(400)]
    public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
    {
        if (registerUser.UserName is null || registerUser.Password is null)
            return BadRequest(new RegisterResponse() { Success = false, TokenJwt = null });
        try
        {
            RegisterResponse registerResponse = await userService.Register(registerUser);
            if (registerResponse.Success)
                return Ok(registerResponse);
            else
                return BadRequest(registerResponse);
        }
        catch (Exception)
        {
            return BadRequest(new RegisterResponse() { Success = false, TokenJwt = null});
        }
    }
    [HttpPost("Login")]
    [ProducesResponseType<LoginResponse>(200)]
    [ProducesResponseType<LoginResponse>(400)]
    public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
    {
        if (loginUser.UserName is null || loginUser.Password is null)
            return BadRequest(new LoginResponse() { Success = false, TokenJwt = null });
        try
        {
            LoginResponse loginResponse = await userService.Login(loginUser);
            if (loginResponse.Success)
                return Ok(loginResponse);
            else
                return BadRequest(loginResponse);
        }
        catch (Exception)
        {
            return BadRequest(new LoginResponse() { Success = false, TokenJwt = null });
        }
    }
    [HttpGet("GetUser")]
    [Authorize]
    [ProducesResponseType<UserResponse>(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUser()
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if(userName is null)
            return Unauthorized(new UserResponse() { UserId = null, UserName = null});
        UserResponse? user = await userService.GetUser(userName);
        if (user is null)
            return Unauthorized(new UserResponse() { UserId = null, UserName = null});
        return Ok(user);
    }
    [HttpPost("UpdateUser")]
    [Authorize]
    [ProducesResponseType<UpdateUserResponse>(200)]
    [ProducesResponseType<UpdateUserResponse>(400)]
    [ProducesResponseType<UpdateUserResponse>(401)]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUser updateUser)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized(new UpdateUserResponse() { Success = false, UserName = null });
        if (userName != updateUser.UserName)
            return BadRequest(new UpdateUserResponse() { Success = false, UserName = null });
        try
        {
            UpdateUserResponse? user = await userService.UpdateUser(updateUser);
            if (user is null)
                return Unauthorized(new UpdateUserResponse() { Success = false, UserName = null });
            if(user.Success)
                return Ok(user);
            return BadRequest(user);
        }
        catch(Exception)
        {
            return BadRequest(new UpdateUserResponse() { Success = false, UserName = null });
        }
    }
    [HttpPost("ChangePassword")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser changePasswordUser)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized();
        if (changePasswordUser.UserName is null || changePasswordUser.OldPassword is null || changePasswordUser.NewPassword is null)
            return BadRequest();
        if (userName != changePasswordUser.UserName)
            return Unauthorized();
        try
        {
            bool isSuccess = await userService.ChangePassword(changePasswordUser);
            if (isSuccess)
                return Ok();
            else
                return BadRequest();
        }
        catch(Exception)
        {
            return BadRequest();
        }
    }
    [HttpPost("DeleteUser")]
    [Authorize]
    [ProducesResponseType<DeleteUserResponse>(200)]
    [ProducesResponseType<DeleteUserResponse>(400)]
    [ProducesResponseType<DeleteUserResponse>(401)]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUser deleteUser)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized(new DeleteUserResponse() { Success = false });
        if (deleteUser.UserName is null)
            return BadRequest(new DeleteUserResponse() { Success = false });
        if (userName != deleteUser.UserName)
            return Unauthorized();
        try
        {
            DeleteUserResponse? user = await userService.DeleteUser(deleteUser);
            if (user is null)
                return BadRequest(new DeleteUserResponse() { Success = false });
            if (user.Success)
                return Ok(user);
            else
                return BadRequest(user);
        }
        catch (Exception)
        {
            return BadRequest(new DeleteUserResponse() { Success = false });
        }
    }
}