namespace Porygon.Entity.Interfaces
{
    public interface IEntityDataManager : IEntityDataManager<PoryEntity, Guid, EntityFilter>        
    {
    }

    public interface IEntityDataManager<T> : IEntityDataManager<T, Guid, EntityFilter>
        where T : IKeyEntity<Guid>
    {
    }

    public interface IEntityDataManager<T, TFilter> : IEntityDataManager<T, Guid, TFilter>
        where T : IKeyEntity<Guid>
        where TFilter : EntityFilter
    {
    }
 
    public interface IEntityDataManager<T, TKey, TFilter> : IBasicEntityDataManager<T, TKey>
        where T : IKeyEntity<TKey>
        where TFilter : EntityFilter<TKey>
    {
        Task<List<T>> Search(TFilter filter);

        Task<T> GetByName(string name);

        Task<List<T>> GetMany(List<TKey> ids);
    }
}