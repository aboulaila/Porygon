using Porygon.Entity.Data;
using Porygon.Identity.Entity;
using System.Security.Claims;

namespace Porygon.Identity.Data
{
    public interface IUserClaimsDataManager : IBasicEntityDataManager<IdentityUserClaim, string>
    {
        public Task<IList<Claim>> GetByUserId(string userId);
        public Task<int> Insert(Claim userClaim, string userId);
        public Task<int> InsertMany(IEnumerable<Claim> claims, string userId);
        public Task<int> Delete(Claim userClaim, string userId);
    }
}
