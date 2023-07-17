namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class Scientist : TrelloEntity
{
    public string FullName { get; set; }

    public IEnumerable<Remark> Remarks { get; set; } = new HashSet<Remark>();
    public IEnumerable<Participant> Participants { get; set; } = new HashSet<Participant>();

    public Scientist(int? id, string? code, string fullName)
	 : base(id, code)
    {
        FullName = fullName;
    }

    public Scientist() 
	 : this(null, null, string.Empty)
    { }
}