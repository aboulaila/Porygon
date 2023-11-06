using Porygon.Entity.Data;
using Porygon.Identity.Entity;

namespace Porygon.Identity.Data
{
    public interface IUserDataManager : IBasicEntityDataManager<User, string>
    {
        public Task<User> GetByName(string userName);
        public Task<User> GetByEmail(string email);
        public Task<List<User>> GetByRole(string roleName);
        public Task<int> Delete(User user);
    }
}
