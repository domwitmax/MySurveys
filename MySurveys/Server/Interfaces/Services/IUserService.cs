using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;

namespace MySurveys.Server.Interfaces.Services;

public interface IUserService
{
    public Task<RegisterResponse> Register(RegisterUser register);
    public Task<LoginResponse> Login(LoginUser login);
    public Task<UserResponse?> GetUser(string username);
    public Task<UpdateUserResponse> UpdateUser(UpdateUser updateUser);
    public Task<bool> ChangePassword(ChangePasswordUser changePasswordUser);
    public Task<DeleteUserResponse> DeleteUser(DeleteUser deleteUser);
}