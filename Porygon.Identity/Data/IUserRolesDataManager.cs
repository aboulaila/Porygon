using Porygon.Entity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.Data
{
    public interface IUserRolesDataManager : IBasicEntityDataManager<IdentityUserRole, string>
    {
        public Task<List<string>> GetByUserId(string userId);
        public Task<int> Insert(string roleId, User user);
        public Task<int> Delete(string roleId, User user);
        public Task<bool> Exists(string roleName, string userId);
    }
}
