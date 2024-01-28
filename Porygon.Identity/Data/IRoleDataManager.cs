using Microsoft.AspNetCore.Identity;
using Porygon.Entity.Interfaces;

namespace Porygon.Identity.Data
{
    public interface IRoleDataManager : IBasicEntityDataManager<IdentityRole, string>
    {
        public Task<IdentityRole> GetByName(string roleName);
    }
}
