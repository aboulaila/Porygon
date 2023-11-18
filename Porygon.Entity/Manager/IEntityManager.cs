namespace Porygon.Entity.Manager
{
    public interface IEntityManager<T, TKey, TFilter, TModel>
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter
        where TModel : T
    {
        Task<T> Create(TModel model);
        Task<T> Update(TModel model);
        Task<int> Delete(TKey id);
        Task<IEnumerable<TModel>> GetAll();
        Task<TModel?> Get(TKey id);
        Task<IEnumerable<TModel>> Search(TFilter filter);
    }
}
