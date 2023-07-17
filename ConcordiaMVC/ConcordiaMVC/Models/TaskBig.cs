namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class TaskBig
{
    public Experiment? Experiment { get; set; }
    public IEnumerable<Scientist>? Assignees { get; set; }
    public IEnumerable<Remark>? Remarks { get; set; }

    public IEnumerable<Scientist>? Scientists { get; set; }
    public IEnumerable<State>? States { get; set; }

    public int SelectedState { get; set; }
    public int SelectedAuthor { get; set; }
    public string Comment { get; set; } = string.Empty;

    public TaskBig(Experiment? experiment,
                   IEnumerable<Scientist>? assignees,
                   IEnumerable<Remark>? remarks,
                   IEnumerable<Scientist>? scientists,
                   IEnumerable<State>? states)
    {
        Experiment = experiment;
        Assignees = assignees;
        Remarks = remarks;
        Scientists = scientists;
        States = states;
    }

    public TaskBig()
    : this(null, new List<Scientist>(), new List<Remark>(), new List<Scientist>(), new List<State>())
    { }

    public TaskBig(Experiment? experiment)
    : this(experiment, new List<Scientist>(), new List<Remark>(), new List<Scientist>(), new List<State>())
    { }

    public TaskBig(Experiment? experiment, IEnumerable<Scientist>? scientists, IEnumerable<State>? states)
    : this(experiment, new List<Scientist>(), new List<Remark>(), scientists, states)
    { }

    public TaskBig(Experiment? experiment, IEnumerable<Remark>? remarks, IEnumerable<Scientist>? scientists, IEnumerable<State>? states)
    : this(experiment, new List<Scientist>(), remarks, scientists, states)
    { }

    public TaskBig(Experiment? experiment, IEnumerable<Scientist>? assignees, IEnumerable<Scientist>? scientists, IEnumerable<State>? states)
    : this(experiment, assignees, new List<Remark>(), scientists, states)
    { }
}