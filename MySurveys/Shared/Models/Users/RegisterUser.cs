namespace MySurveys.Shared.Models.Users;

public class RegisterUser
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public RegisterUser(string userName, string password)
    {
        UserName = userName; 
        Password = password;
    }
}