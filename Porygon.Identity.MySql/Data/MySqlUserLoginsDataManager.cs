using Microsoft.AspNetCore.Identity;
using Porygon.Entity.MySql.Data;
using Porygon.Identity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.MySql.Data
{
    public class MySqlUserLoginsDataManager : MySqlBasicEntityDataManager<IdentityUserLogin, string>, IUserLoginsDataManager
    {
        public MySqlUserLoginsDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<int> Delete(IdentityUser user, string loginProvider, string providerKey)
        {
            return await Connection.Delete<IdentityUserLogin>()
                .Where(x => x.UserId.Equals(user.Id)
                    && x.LoginProvider.Equals(loginProvider)
                    && x.ProviderKey.Equals(providerKey))
                .ExecuteAffrowsAsync();
        }

        public async Task<IList<UserLoginInfo>> GetByUserId(string userId)
        {
            List<UserLoginInfo> logins = await Connection.Select<IdentityUserLogin>()
                .Where(x => x.UserId.Equals(userId))
                .ToListAsync(r => new UserLoginInfo(r.LoginProvider, r.ProviderKey, null));

            return logins;
        }

        public async Task<int> Insert(UserLoginInfo login, IdentityUser user)
        {
            return await InsertAsync(new IdentityUserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id,
            });
        }
    }
}
