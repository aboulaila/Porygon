namespace Porygon.Entity.Data
{
    public interface IBasicEntityDataManager<T, TKey>
        where T : IKeyEntity<TKey>
    {
        T? Get(TKey id);

        Task<T?> GetAsync(TKey id);

        Task<List<T>> GetAll();

        int Insert(T entity);

        Task<int> InsertAsync(T entity);

        int InsertMany(IEnumerable<T> entities);

        Task<int> InsertManyAsync(IEnumerable<T> entities);

        int Update(T entity);

        Task<int> UpdateAsync(T entity);

        int Delete(TKey id);

        Task<int> DeleteAsync(TKey id);
    }

    public interface IBasicEntityDataManager<T> : IBasicEntityDataManager<T, Guid>
        where T : IKeyEntity<Guid>
    {
    }
}
