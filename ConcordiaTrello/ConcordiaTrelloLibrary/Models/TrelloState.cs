namespace ConcordiaTrelloLibrary.Models;

public class TrelloState 
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public TrelloState(string code, string name)
    {
        Code = code;
        Name = name;
    }
}