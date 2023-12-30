using Porygon.Entity.Data;

namespace Porygon.Entity.MySql.Data
{
    public class MySqlBasicEntityDataManager<T> : MySqlBasicEntityDataManager<T, Guid>
        where T : class
    {
        protected MySqlBasicEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public class MySqlBasicEntityDataManager<T, TKey> : IBasicEntityDataManager<T, TKey>
    where T : class
    {
        protected IFreeSql Connection;

        public MySqlBasicEntityDataManager(IFreeSql connection)
        {
            Connection = connection;
        }

        public T? Get(TKey id)
        {
            return Connection.Select<T>(id).First();
        }

        public async Task<T?> GetAsync(TKey id)
        {
            return await Connection.Select<T>(id).FirstAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return await Connection.Select<T>().ToListAsync();
        }

        public int Insert(T entity)
        {
            return Connection.Insert(entity).ExecuteAffrows();
        }

        public async Task<int> InsertAsync(T entity)
        {
            return await Connection.Insert(entity).ExecuteAffrowsAsync();
        }

        public int InsertMany(IEnumerable<T> entities)
        {
            return Connection.Insert(entities).ExecuteAffrows();
        }

        public async Task<int> InsertManyAsync(IEnumerable<T> entities)
        {
            return await Connection.Insert(entities).ExecuteAffrowsAsync();
        }

        public int Update(T entity)
        {
            return Connection.Update<T>().SetSource(entity).ExecuteAffrows();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            return await Connection.Update<T>().SetSource(entity).ExecuteAffrowsAsync();
        }

        public virtual int Delete(TKey id)
        {
            return Connection.Delete<T>(new { id }).ExecuteAffrows();
        }

        public async Task<int> DeleteAsync(TKey id)
        {
            return await Connection.Delete<T>(new { id }).ExecuteAffrowsAsync();
        }
    }
}
