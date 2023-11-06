using Microsoft.Extensions.DependencyInjection;
using Porygon.Identity.Data;
using Porygon.Identity.MySql.Data;

namespace Porygon.Identity.MySql
{
    public static class Extensions
    {
        public static IServiceCollection AddMySqlIdentity(this IServiceCollection services)
        {
            services.AddCustomIdentity();

            services.AddScoped<IUserDataManager, MySqlUserDataManager>();
            services.AddScoped<IUserClaimsDataManager, MySqlUserClaimsDataManager>();
            services.AddScoped<IRoleDataManager, MySqlRoleDataManager>();
            services.AddScoped<IUserRolesDataManager, MySqlUserRolesDataManager>();
            services.AddScoped<IUserLoginsDataManager, MySqlUserLoginsDataManager>();

            return services;
        }
    }
}
