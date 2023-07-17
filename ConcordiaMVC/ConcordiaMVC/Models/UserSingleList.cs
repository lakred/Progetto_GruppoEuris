namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class UserSingleList
{
    public Scientist Scientist { get; set; }
    public IEnumerable<Experiment>? Experiments { get; set; }

    public UserSingleList(Scientist scientist, IEnumerable<Experiment>? experiments)
    {
        Scientist = scientist;
        Experiments = experiments;
    }

    public UserSingleList(Scientist scientist)
    : this(scientist, new List<Experiment>())
    {
    }
}