namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class UserMultiList
{
    public Scientist Scientist { get; set; }
    public IEnumerable<Experiment>? ExperimentsInStart { get; set; }
    public IEnumerable<Experiment>? ExperimentsInWorking { get; set; }
    public IEnumerable<Experiment>? ExperimentsInFinish { get; set; }


    public UserMultiList(Scientist scientist,
                         IEnumerable<Experiment>? experimentsInStart,
                         IEnumerable<Experiment>? experimentsInWorking,
                         IEnumerable<Experiment>? experimentsInFinish)
    {
        Scientist = scientist;
        ExperimentsInStart = experimentsInStart;
        ExperimentsInWorking = experimentsInWorking;
        ExperimentsInFinish = experimentsInFinish;
    }

    public UserMultiList(Scientist scientist)
    : this(scientist, new List<Experiment>(), new List<Experiment>(), new List<Experiment>())
    { }
}