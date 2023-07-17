namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class Experiment : TrelloEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Loaded { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public int PriorityId { get; set; }
    public int StateId { get; set; }

    public Priority? Priority { get; set; } = null;
    public State? State { get; set; } = null;

    public IEnumerable<Remark> Remarks { get; set; } = new HashSet<Remark>();
    public IEnumerable<Participant> Participants { get; set; } = new HashSet<Participant>();


    public Experiment(int? id, string? code, string name, string description,
                      bool loaded, DateTimeOffset? startDate, DateTimeOffset? dueDate,
                      int priorityId, int stateId)
     : base(id, code)
    {
        Name = name;
        Description = description;
        Loaded = loaded;
        StartDate = startDate;
        DueDate = dueDate;
        PriorityId = priorityId;
        StateId = stateId;
    }

    public Experiment() 
	 : this(null, null, string.Empty, string.Empty, false, null, null, 0, 0)
    { }

    public static bool FromBitToBoll(byte value)
    {
        return !(value == 0);
    }

    public static byte FromBoolToBit(bool value)
    {
        return value ? (byte)1 : (byte)0;
    }
}