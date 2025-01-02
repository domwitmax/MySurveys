namespace MySurveys.Shared;

public class OptionQuestion
{
    public readonly QuestionEnum Type;
    public readonly IEnumerable<string>? Choices;
    public readonly string? Path;
    public readonly int? ImageWidth;
    public readonly int? ImageHeight;
    public OptionQuestion(QuestionEnum type)
    {
        Type = type;
    }
    public OptionQuestion(QuestionEnum type, IEnumerable<string>? choices)
    {
        Type = type;
        Choices = choices;
    }
    public OptionQuestion(QuestionEnum type, string path, int imageWidth, int imageHeight)
    {
        Type = type;
        Path = path;
        ImageWidth = imageWidth;
        ImageHeight = imageHeight;
    }
}