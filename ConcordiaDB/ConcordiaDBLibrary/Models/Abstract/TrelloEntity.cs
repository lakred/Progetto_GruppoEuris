namespace ConcordiaDBLibrary.Models.Abstract;

public abstract class TrelloEntity : Entity
{
    public string? Code { get; set; } = null;

    public TrelloEntity(int? id = default, string? code = default) : base(id)
    {
        Code = code;
    }
}