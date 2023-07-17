namespace ConcordiaTrelloLibrary.Models;

public class TrelloScientist 
{
    public string Code { get; set; } = null!;
    public string FullName { get; set; }

    public TrelloScientist(string code, string fullname)
    {
        Code = code;
        FullName = fullname;
    }
}