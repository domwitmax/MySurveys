namespace MySurveys.Shared;

public class OptionQuestion
{
    public QuestionEnum Type { get; set; }
    public IEnumerable<string>? Choices { get; set; }
    public string? Path { get; set; }
    public int? ImageWidth { get; set; }
    public int? ImageHeight { get; set; }
    public OptionQuestion()
    {

    }
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