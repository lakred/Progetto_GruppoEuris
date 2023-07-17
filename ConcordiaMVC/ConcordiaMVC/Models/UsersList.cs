namespace ConcordiaMVC.Models;

using ConcordiaDBLibrary.Models.Classes;

public class UsersList
{
    public IEnumerable<Scientist>? Scientists { get; set; }

    public UsersList(IEnumerable<Scientist>? scientists)
    {
        Scientists = scientists;
    }

    public UsersList()
    : this(new List<Scientist>())
    { }
}