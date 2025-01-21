namespace MySurveys.Shared.Models.Responses;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string? TokenJwt { get; set; }
}