namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class State : TrelloEntity
{
    public string Name { get; set; }

    public IEnumerable<Experiment> Experiments { get; set; } = new HashSet<Experiment>();

    public State(int? id, string? code, string name) : base(id, code)
    {
        Name = name;
    }

    public State()
     : this(null, null, string.Empty)
    { }
}