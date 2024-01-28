namespace Porygon.Entity.Interfaces
{
    public interface IBasicEntityDataManager<T, TKey> : IDataManager
    {
        T? Get(TKey id);

        Task<int> InsertAsync(T entity);

        int InsertMany(IEnumerable<T> entities);

        Task<int> InsertManyAsync(IEnumerable<T> entities);

        Task<int> UpdateAsync(T entity);

        Task<int> DeleteAsync(TKey id);
    }

    public interface IBasicEntityDataManager<T> : IBasicEntityDataManager<T, Guid>
    {
    }
}
