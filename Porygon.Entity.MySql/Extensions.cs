using Microsoft.Extensions.DependencyInjection;
using Porygon.Entity.Interfaces;
using Porygon.Entity.MySql.Data;

namespace Flapple.DataAccess.MySQL
{
    public static class Extensions
    {
        public static IServiceCollection AddFreeSql(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(x => new FreeSql.FreeSqlBuilder()
              .UseConnectionString(FreeSql.DataType.MySql, connectionString)
              .UseAutoSyncStructure(true) //automatically synchronize the entity structure to the database
              .Build());

            services.AddSingleton<IDbConnectionProvider, MySqlDbConnectionProvider>();
            return services;
        }
    }
}
