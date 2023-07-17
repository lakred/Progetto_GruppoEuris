namespace ConcordiaTrelloLibrary.Models;

public class TrelloRemark 
{
    public string Code { get; set; } = null!;
    public string Text { get; set; }
    public DateTimeOffset? Date { get; set; }
    public TrelloScientist TScientist { get; set; }

    public TrelloRemark(string code, string text, DateTimeOffset? date, TrelloScientist scientist)
    {
        Code = code;
        Text = text;
        Date = date;
        TScientist = scientist;
    }
}