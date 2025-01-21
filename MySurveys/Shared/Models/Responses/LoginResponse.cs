namespace MySurveys.Shared.Models.Responses;

public class LoginResponse
{
    public bool Success { get; set; }
    public string? TokenJwt { get; set; }
}