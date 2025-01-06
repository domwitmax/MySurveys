using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;

namespace MySurveys.Server.Interfaces.Services;

public interface IUserService
{
    public RegisterResponse Register(RegisterUser register);
    public LoginResponse Login(LoginUser login);
    public UserResponse GetUser(string username);
    public UpdateUserResponse UpdateUser(UpdateUser updateUser);
    public DeleteUserResponse DeleteUser(DeleteUser deleteUser);
}