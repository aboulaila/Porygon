using Porygon.Entity.Entity;

namespace Porygon.Entity.Utils
{
    public static class EntityHelper
    {

        public static List<I> GetEntityInstances<T, I>(this T entity, object? relatedEntity)
        {
            if (relatedEntity is I entityInstance)
                return new List<I>() { entityInstance };
            else if (relatedEntity is IEnumerable<I> entityList)
                return entityList.FilterNulls();
            throw new InvalidCastException($"EntityInstance related to {entity!.GetType().Name}, doesn't have the correct type");
        }

        public static bool ShouldCreateEntity<TKey>(this IKeyEntity<Guid> entity)
        {
            return entity?.State == EntityStates.NEW || EmptyId(entity!.Id);
        }

        public static bool EmptyId(object? id)
        {
            if (id == null)
                return true;
            if (id is Guid guid)
                return guid.Equals(Guid.Empty);
            else if (id is object obj)
                return obj == default;
            return true;
        }
    }
}

