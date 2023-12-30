using System.Data;

namespace Porygon.Entity.Manager
{
    public interface IEntityManager
    {
        Task<object?> Create(object model, IDbTransaction transaction);
        Task<object?> Update(object model, IDbTransaction transaction);
        Task<int> Delete(object id, IDbTransaction transaction);
        Task<object?> Get(object id);
        Task<IEnumerable<object?>> GetByLinkedItemId(object id);
    }

    public interface IEntityManager<T, TKey, TFilter, TModel> : IEntityManager
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter<TKey>
        where TModel : T
    {
        Task<T?> Create(TModel model);
        Task<T?> Update(TModel model);
        Task<int> Delete(TKey id);
        Task<TModel?> Get(TKey id);
        Task<TModel?> GetEnriched(TKey id);
        Task<List<TModel>> GetAll();
        Task<List<TModel>> GetAllEnriched();
        Task<List<TModel>> Search(TFilter filter);
        Task<List<TModel>> SearchEnriched(TFilter filter);
    }
}
