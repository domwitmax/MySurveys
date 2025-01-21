namespace MySurveys.Shared.Models.Users;

public class LoginUser
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public LoginUser(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}