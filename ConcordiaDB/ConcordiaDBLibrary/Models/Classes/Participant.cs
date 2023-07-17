namespace ConcordiaDBLibrary.Models.Classes;

using Abstract;

public class Participant : Entity
{
    public int ExperimentId { get; set; }
    public int? ScientistId { get; set; }

    public Experiment? Experiment { get; set; } = null;
    public Scientist? Scientist { get; set; } = null;

    public Participant(int? id, int experimentId, int? scientistId)
	 : base(id)
    {
        ExperimentId = experimentId;
        ScientistId = scientistId;
    }

    public Participant() 
	 : this(null, 0, 0)
    { }
}