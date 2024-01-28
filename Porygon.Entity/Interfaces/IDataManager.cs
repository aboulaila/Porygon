using System.Data;

namespace Porygon.Entity.Interfaces
{
    public interface IDataManager
    {
        Task<R?> GetAsync<R>(object id) where R : class;
        Task<List<R>> GetAll<R>();
        int Insert(object entity);
        int Insert(object entity, IDbTransaction transaction);
        int Update(object entity);
        int Update(object entity, IDbTransaction transaction);
        int Delete(object id);
        int Delete(object id, IDbTransaction transaction);
    }
}

