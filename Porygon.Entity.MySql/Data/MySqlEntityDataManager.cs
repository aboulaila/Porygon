﻿using Porygon.Entity.Interfaces;

namespace Porygon.Entity.MySql.Data
{
    public class MySqlEntityDataManager : MySqlEntityDataManager<PoryEntity, EntityFilter>, IEntityDataManager
    {
        public MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public class MySqlEntityDataManager<T> : MySqlEntityDataManager<T, EntityFilter>, IEntityDataManager<T>
        where T : PoryEntity
    {
        public MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public class MySqlEntityDataManager<T, TFilter> : MySqlEntityDataManager<T, Guid, TFilter>, IEntityDataManager<T, TFilter>
        where T : PoryEntity
        where TFilter : EntityFilter
    {
        public MySqlEntityDataManager(IFreeSql connection) : base(connection)
        {
        }
    }

    public class MySqlEntityDataManager<T, TKey, TFilter> : MySqlBasicEntityDataManager<T, TKey>, IEntityDataManager<T, TKey, TFilter>
        where T : PoryEntity<TKey>
        where TFilter : EntityFilter<TKey>
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

        public async Task<List<T>> GetMany(List<TKey> ids)
        {
            return await Connection.Select<T>()
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }

        public async virtual Task<List<T>> Search(TFilter filter)
        {
            return await Connection.Select<T>(filter)
                .Where(e =>
                    string.IsNullOrEmpty(filter.Criteria)
                    || $"{e.Title}".StartsWith(filter.Criteria, StringComparison.OrdinalIgnoreCase)
                   )
                .Page(filter.Skip, filter.Take == 0 ? 20 : filter.Take)
                .OrderBy(e => e.DateCreated)
                .ToListAsync();
        }
    }
}
