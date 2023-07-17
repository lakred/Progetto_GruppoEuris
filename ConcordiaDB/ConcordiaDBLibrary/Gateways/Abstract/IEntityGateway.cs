namespace ConcordiaDBLibrary.Gateways.Abstract;

using Models.Abstract;

public interface IEntityGateway<TEntity> where TEntity : Entity
{
    public IEnumerable<TEntity> GetAll();

    public TEntity? GetById(int id);
    public IEnumerable<TEntity>? GetByIdMulti(IEnumerable<int>? ids);

    public TEntity Insert(TEntity entity);
    public IEnumerable<TEntity>? InsertMulti(IEnumerable<TEntity>? entities);

    public TEntity Update(TEntity entity);
    public IEnumerable<TEntity>? UpdateMulti(IEnumerable<TEntity>? entities);

    public TEntity Delete(int id);
    public IEnumerable<TEntity>? DeleteMulti(IEnumerable<int>? ids);
}