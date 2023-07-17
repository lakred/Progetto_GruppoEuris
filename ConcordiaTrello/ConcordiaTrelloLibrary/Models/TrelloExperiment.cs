namespace ConcordiaTrelloLibrary.Models;

public class TrelloExperiment 
{
    public string Code { get; set; } = null!;
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public TrelloPriority TPriority { get; set; }
    public TrelloState TState { get; set; }
    public IEnumerable<TrelloScientist>? TScientists { get; set; }
    public List<TrelloRemark>? TRemarks { get; set; }

    public TrelloExperiment(string code, string name, string description,
                       DateTimeOffset? dueDate, DateTimeOffset? startDate,
                       TrelloPriority priority, TrelloState state,
                       IEnumerable<TrelloScientist>? scientist, List<TrelloRemark>? remarks)
    {
        Code = code;
        Name = name;
        Description = description;
        DueDate = dueDate;
        StartDate = startDate;
        TPriority = priority;
        TState = state;
        TScientists = scientist;
        TRemarks = remarks;
    }
}