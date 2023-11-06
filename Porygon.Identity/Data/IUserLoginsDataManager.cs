using Microsoft.AspNetCore.Identity;
using Porygon.Entity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.Data
{
    public interface IUserLoginsDataManager : IBasicEntityDataManager<IdentityUserLogin, string>
    {
        public Task<int> Delete(IdentityUser user, string loginProvider, string providerKey);
        public Task<int> Insert(UserLoginInfo login, IdentityUser user);
        public Task<IList<UserLoginInfo>> GetByUserId(string userId);
    }
}
