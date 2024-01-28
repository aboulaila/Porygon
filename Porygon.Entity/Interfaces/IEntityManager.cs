namespace Porygon.Entity.Interfaces
{
    public interface IEntityManager
    {
        IDataManager GetDataManager();
        IEntityEventsHook GetCreationHook();
        IEntityEventsHook GetUpdateHook();
        IEntityEventsHook GetDeletionHook();
        Task<IEnumerable<object?>> GetLinkedItems(Guid id);
        Task<object?> GetSingle(Guid id);
    }

    public interface IEntityManager<T, TFilter, TModel> : IEntityManager
        where T : IKeyEntity<Guid>
        where TFilter : EntityFilter
        where TModel : T
    {
        Task<T?> Create(TModel model);
        Task<T?> Update(TModel model);
        Task<int> Delete(Guid id);
        Task<TModel?> Get(Guid id);
        Task<TModel?> GetEnriched(Guid id);
        Task<List<TModel>> GetAll();
        Task<List<TModel>> GetAllEnriched();
        Task<List<TModel>> Search(TFilter filter);
        Task<List<TModel>> SearchEnriched(TFilter filter);
        Task<List<TModel>> GetByLinkedItemId(Guid id);
    }
}
