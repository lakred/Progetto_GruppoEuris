namespace ConcordiaTrelloLibrary.Models;

public class TrelloPriority 
{
    public string Code { get; set; } = null!;
    public string Name { get; set; }
    public string Color { get; set; }

    public TrelloPriority(string code, string name, string color)
    {
        Code = code;
        Name = name;
        Color = color;
    }
}