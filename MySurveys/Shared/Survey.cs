namespace MySurveys.Shared;

public class Survey
{
    public int Id { get; set; }
    public HeaderQuestion[] Headers { get; set; }
    public OptionQuestion[] Options { get; set; }
    public Survey()
    {}
}