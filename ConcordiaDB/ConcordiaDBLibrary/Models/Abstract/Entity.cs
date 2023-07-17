namespace ConcordiaDBLibrary.Models.Abstract;

public abstract class Entity
{
	public int? Id { get; set; }

	public Entity(int? id = default)
	{
		Id = id;
	}
}

