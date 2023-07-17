namespace ConcordiaDBLibrary.Models.Extensions;

using Models.Abstract;

public class EntityEqualityComparer<TEntity> : IEqualityComparer<TEntity> where TEntity : Entity
{
    public bool Equals(TEntity? entityX, TEntity? entityY)
    {
        if (ReferenceEquals(entityX, entityY))
        {
            return true;
        }
        if (entityX is null || entityY is null)
        {
            return false;
        }
        if (entityX.GetType() != entityY.GetType())
        {
            return false;
        }

        return entityX.Id == entityY.Id;
    }

    public int GetHashCode(TEntity entity)
    {
        return entity.Id.GetHashCode();
    }
}