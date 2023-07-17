namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class Priority : TrelloEntity
{
    public string Name { get; set; }
    public string Color { get; set; }

    public IEnumerable<Experiment> Experiments { get; set; } = new HashSet<Experiment>();

    public Priority(int? id, string? code, string name, string color)
	 : base(id, code)
    {
        Code = code;
        Name = name;
        Color = color;
    }

    public Priority() 
	 : this(null, null, string.Empty, string.Empty)
    { }
}