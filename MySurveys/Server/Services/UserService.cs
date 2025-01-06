using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Responses;
using MySurveys.Shared.Models.Users;

namespace MySurveys.Server.Services;

public class UserService : IUserService
{
    public UserService()
    {

    }
    public DeleteUserResponse DeleteUser(DeleteUser deleteUser)
    {
        throw new NotImplementedException();
    }

    public UserResponse GetUser(string username)
    {
        throw new NotImplementedException();
    }

    public LoginResponse Login(LoginUser login)
    {
        throw new NotImplementedException();
    }

    public RegisterResponse Register(RegisterUser register)
    {
        throw new NotImplementedException();
    }

    public UpdateUserResponse UpdateUser(UpdateUser updateUser)
    {
        throw new NotImplementedException();
    }
}
