using System.Data;
using Porygon.Entity.Interfaces;

namespace Porygon.Entity.MySql.Data
{
    public abstract class MySqlDataManager : IDataManager
    {
        protected IFreeSql Connection;
        public MySqlDataManager(IFreeSql connection)
        {
            Connection = connection;
        }

        public async Task<R?> GetAsync<R>(object id)
            where R : class
        {
            return await Connection.Select<R>(id).FirstAsync();
        }

        public abstract Task<List<R>> GetAll<R>();
        public abstract int Delete(object id);
        public abstract int Delete(object id, IDbTransaction transaction);
        public abstract int Insert(object entity);
        public abstract int Insert(object entity, IDbTransaction transaction);
        public abstract int Update(object entity);
        public abstract int Update(object entity, IDbTransaction transaction);
    }
}

