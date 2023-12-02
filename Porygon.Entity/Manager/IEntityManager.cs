using System.Transactions;

namespace Porygon.Entity.Manager
{
    public interface IEntityManager
    {
        Task<object?> Create(object model, TransactionScope scope);
        Task<object?> Update(object model, TransactionScope scope);
        Task<int> Delete(object id, TransactionScope scope);
    }

    public interface IEntityManager<T, TKey, TFilter, TModel> : IEntityManager
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter
        where TModel : T
    {
        Task<T?> Create(TModel model);
        Task<T?> Update(TModel model);
        Task<int> Delete(TKey id);
        Task<IEnumerable<TModel>> GetAll();
        Task<TModel?> Get(TKey id);
        Task<IEnumerable<TModel>> Search(TFilter filter);
    }
}
