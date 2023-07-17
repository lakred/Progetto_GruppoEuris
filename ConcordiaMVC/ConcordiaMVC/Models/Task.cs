namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class Task
{
    public Experiment Experiment { get; set; }
    public Remark? LastRemark { get; set; }
    public string Text => LastRemark is null ? string.Empty : LastRemark.Text;

    public Task(Experiment experiment, Remark? lastremark)
    {
        Experiment = experiment;
        LastRemark = lastremark;
    }

    public Task(Experiment experiment)
    : this(experiment, null)
    { }

}