namespace Porygon.Entity.Data
{
    public interface IEntityDataManager : IEntityDataManager<PoryEntity, Guid, EntityFilter>        
    {
    }

    public interface IEntityDataManager<T> : IEntityDataManager<T, Guid, EntityFilter>
        where T : PoryEntity<Guid>
    {
    }

    public interface IEntityDataManager<T, TFilter> : IEntityDataManager<T, Guid, TFilter>
        where T : PoryEntity<Guid>
        where TFilter : EntityFilter
    {
    }
 
    public interface IEntityDataManager<T, TKey, TFilter> : IBasicEntityDataManager<T, TKey>
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter
    {
        Task<IEnumerable<T>> Search(TFilter filter);

        Task<T> GetByName(string name);

        Task<IEnumerable<T>> GetMany(List<TKey> ids);
    }
}