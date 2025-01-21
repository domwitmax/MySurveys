namespace MySurveys.Server.Data.Entity;

public class OptionImageEntity
{
    public int Id { get; set; }
    public string Path { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public OptionImageEntity(string path, int width, int height)
    {
        Path = path; 
        Width = width; 
        Height = height;
    }
}