using Microsoft.AspNetCore.Identity;
using Porygon.Entity.MySql.Data;
using Porygon.Identity.Data;

namespace Porygon.Identity.MySql.Data
{
    public class MySqlRoleDataManager : MySqlBasicEntityDataManager<IdentityRole, string>, IRoleDataManager
    {
        public MySqlRoleDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<IdentityRole> GetByName(string roleName)
        {
            return await Connection.Select<IdentityRole>()
                .Where(x => Equals(x.Name, roleName))
                .FirstAsync();
        }
    }
}
