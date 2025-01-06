namespace MySurveys.Shared.Models.Questions;

public class Survey
{
    public int Id { get; set; }
    public HeaderQuestion[] Headers { get; set; }
    public OptionQuestion[] Options { get; set; }
    public Survey()
    { }
}