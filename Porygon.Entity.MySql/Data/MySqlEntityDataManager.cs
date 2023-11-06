using Porygon.Entity.Data;

namespace Porygon.Entity.MySql.Data
{
    public abstract class MySqlEntityDataManager<T> : MySqlEntityDataManager<T, EntityFilter>
        where T : PoryEntity
    {
        protected MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public abstract class MySqlEntityDataManager<T, TFilter> : MySqlEntityDataManager<T, Guid, TFilter>
        where T : PoryEntity
        where TFilter : EntityFilter
    {
        protected MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public abstract class MySqlEntityDataManager<T, TKey, TFilter> : MySqlBasicEntityDataManager<T, TKey>, IEntityDataManager<T, TKey, TFilter>
    where T : PoryEntity<TKey>
    where TFilter : EntityFilter
    {
        public MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<T> GetByName(string name)
        {
            return await Connection.Select<T>()
                .Where(x => Equals(x.Name == name, StringComparison.InvariantCultureIgnoreCase))
                .FirstAsync();
        }

        public async Task<IEnumerable<T>> GetMany(List<TKey> ids)
        {
            return await Connection.Select<T>()
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }

        public async virtual Task<IEnumerable<T>> Search(TFilter filter)
        {
            return await Connection.Select<T>(filter)
                .Where(e =>
                    string.IsNullOrEmpty(filter.Criteria)
                    || $"{e.Title}".Contains(filter.Criteria)
                   )
                .Page(filter.Skip, filter.Take)
                //.OrderBy(e => e.DateCreated)
                .ToListAsync();
        }
    }
}
