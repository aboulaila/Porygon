using Microsoft.AspNetCore.Identity;
using Porygon.Entity.MySql.Data;
using Porygon.Identity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.MySql.Data
{
    public class MySqlUserRolesDataManager : MySqlBasicEntityDataManager<IdentityUserRole, string>, IUserRolesDataManager
    {
        public MySqlUserRolesDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<int> Delete(string roleId, User user)
        {
            return await Connection.Delete<IdentityUserRole>()
                .Where(x => Equals(roleId, x.RoleId)
                    && Equals(user.Id, x.UserId))
                .ExecuteAffrowsAsync();
        }

        public async Task<List<string>> GetByUserId(string userId)
        {
            return await Connection.Select<IdentityUserRole, IdentityRole>()
                .LeftJoin(w => w.t1.RoleId == w.t2.Id)
                .Where(w => Equals(userId, w.t1.UserId.ToString()))
                .ToListAsync(w => w.t2.Name ?? string.Empty);
        }

        public async Task<int> Insert(string roleId, User user)
        {
            return await InsertAsync(new IdentityUserRole
            {
                RoleId = roleId,
                UserId = user.Id
            });
        }

        public async Task<bool> Exists(string roleName, string userId)
        {
            return await Connection.Select<IdentityUserRole, IdentityRole>()
                .LeftJoin(w => w.t1.RoleId == w.t2.Id)
                .Where(w => Equals(userId, w.t1.UserId.ToString())
                    && Equals(roleName, w.t2.Name.ToString()))
                .CountAsync() > 0;
        }
    }
}
