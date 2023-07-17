namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class TasksList
{
    public IEnumerable<Task>? Tasks { get; set; }

    public TasksList(IEnumerable<Task>? tasks)
    {
        Tasks = tasks;
    }

    public TasksList()
    : this(new List<Task>())
    { }
}