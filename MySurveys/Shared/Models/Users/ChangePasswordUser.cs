namespace MySurveys.Shared.Models.Users;

public class ChangePasswordUser
{
    public string UserName { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
}