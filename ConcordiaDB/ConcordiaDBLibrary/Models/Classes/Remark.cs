namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class Remark : TrelloEntity
{
    public string Text { get; set; }
    public DateTimeOffset? Date { get; set; }
    public int ExperimentId { get; set; }
    public int ScientistId { get; set; }

    public Experiment? Experiment { get; set; } = null;
    public Scientist? Scientist { get; set; } = null;

    public Remark(int? id, string? code, string text, DateTimeOffset? date, 
	              int experimentId, int scientistId)
	 : base(id, code)
    {
        Text = text;
        Date = date;
        ExperimentId = experimentId;
        ScientistId = scientistId;
    }

    public Remark()
	 : this(null, null, string.Empty, null, 0, 0)
    { }
}