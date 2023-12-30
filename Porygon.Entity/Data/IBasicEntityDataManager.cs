using System.Data;

namespace Porygon.Entity.Data
{
    public interface IBasicEntityDataManager<T, TKey>
        where T : class
    {
        T? Get(TKey id);

        Task<T?> GetAsync(TKey id);

        Task<List<T>> GetAll();

        int Insert(T entity);

        int Insert(T entity, IDbTransaction transaction);

        Task<int> InsertAsync(T entity);

        int InsertMany(IEnumerable<T> entities);

        Task<int> InsertManyAsync(IEnumerable<T> entities);

        int Update(T entity);

        int Update(T entity, IDbTransaction transaction);

        Task<int> UpdateAsync(T entity);

        int Delete(TKey id);

        int Delete(TKey id, IDbTransaction transaction);

        Task<int> DeleteAsync(TKey id);
    }

    public interface IBasicEntityDataManager<T> : IBasicEntityDataManager<T, Guid>
        where T : class
    {
    }
}
