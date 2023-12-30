using System.Data;
using Porygon.Entity.Data;

namespace Porygon.Entity.MySql.Data
{
    public class MySqlDbConnectionProvider : IDbConnectionProvider
    {
        private readonly IFreeSql FreeSql;
		public MySqlDbConnectionProvider(IFreeSql freeSql)
		{
            FreeSql = freeSql;
		}

		public IDbConnection GetConnection()
        {
            return FreeSql.Ado.MasterPool.Get().Value;
        }
    }
}

