using Microsoft.AspNetCore.Identity;
using Porygon.Entity.MySql.Data;
using Porygon.Identity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.MySql.Data
{
    public class MySqlUserDataManager : MySqlBasicEntityDataManager<User, string>, IUserDataManager
    {
        public MySqlUserDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<int> Delete(User user)
        {
            return await DeleteAsync(user.Id);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await Connection
                .Select<User>()
                .Where(x => Equals(x.Email, email))
                .FirstAsync();
        }

        public async Task<User> GetByName(string userName)
        {
            return await Connection
                .Select<User>()
                .Where(x => Equals(x.UserName, userName))
                .FirstAsync();
        }

        public async Task<List<User>> GetByRole(string roleName)
        {
            return await Connection.Select<IdentityUserRole, IdentityRole, User>()
                .LeftJoin(w => w.t1.RoleId == w.t2.Id)
                .Where(w => Equals(roleName, w.t2.Name.ToString()))
                .ToListAsync(w => w.t3);
        }
    }
}
