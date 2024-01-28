using System.Data;
using System.Data.Common;
using Porygon.Entity.Interfaces;

namespace Porygon.Entity.MySql.Data
{
    public class MySqlBasicEntityDataManager<T> : MySqlBasicEntityDataManager<T, Guid>
        where T : class
    {
        protected MySqlBasicEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public class MySqlBasicEntityDataManager<T, TKey> : MySqlDataManager, IBasicEntityDataManager<T, TKey>
        where T : class
    {

        public MySqlBasicEntityDataManager(IFreeSql connection) : base(connection)
        {
        }

        public T? Get(TKey id)
        {
            return Connection.Select<T>(id).First();
        }

        public override async Task<List<R>> GetAll<R>()
        {
            return await Connection.Select<T>().ToListAsync<R>();
        }

        public override int Insert(object entity)
        {
            return Connection.Insert((T)entity).ExecuteAffrows();
        }

        public override int Insert(object entity, IDbTransaction transaction)
        {
            return Connection.Insert((T)entity).WithTransaction((DbTransaction)transaction).ExecuteAffrows();
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

        public override int Update(object entity)
        {
            return Connection.Update<T>().SetSourceIgnore((T)entity, IgnoreColumn).ExecuteAffrows();
        }

        public override int Update(object entity, IDbTransaction transaction)
        {
            return Connection.Update<T>().SetSourceIgnore((T)entity, IgnoreColumn).WithTransaction((DbTransaction)transaction).ExecuteAffrows();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            return await Connection.Update<T>().SetSourceIgnore(entity, IgnoreColumn).ExecuteAffrowsAsync();
        }

        public override int Delete(object id)
        {
            return Connection.Delete<T>(new { id = (TKey)id }).ExecuteAffrows();
        }

        public override int Delete(object id, IDbTransaction transaction)
        {
            return Connection.Delete<T>(new { id = (TKey)id }).WithTransaction((DbTransaction)transaction).ExecuteAffrows();
        }

        public async Task<int> DeleteAsync(TKey id)
        {
            return await Connection.Delete<T>(new { id }).ExecuteAffrowsAsync();
        }

        private bool IgnoreColumn(object column)
        {
            if (column == null)
                return true;

            if (column is DateTime date)
                return date == default;

            if (column is Guid id)
                return id == Guid.Empty;

            return false;
        }
    }
}
