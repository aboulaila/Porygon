using Porygon.Entity.MySql.Data;
using Porygon.Identity.Data;
using Porygon.Identity.Entity;
using System.Security.Claims;

namespace Porygon.Identity.MySql.Data
{
    public class MySqlUserClaimsDataManager : MySqlBasicEntityDataManager<IdentityUserClaim, string>, IUserClaimsDataManager
    {
        public MySqlUserClaimsDataManager(IFreeSql connection) : base(connection)
        {
        }

        public async Task<int> Delete(Claim userClaim, string userId)
        {
            return await Connection.Delete<IdentityUserClaim>()
                .Where(x => x.ClaimValue.Equals(userClaim.Value)
                    && x.ClaimType.Equals(userClaim.Type)
                    && x.UserId.Equals(userId))
                .ExecuteAffrowsAsync();
        }

        public async Task<IList<Claim>> GetByUserId(string userId)
        {
            var results = await Connection.Select<IdentityUserClaim>()
                .Where(x => x.UserId.Equals(userId))
                .ToListAsync();

            return results.Where(r => r != null)
                .Select(r => new Claim(r.ClaimType, r.ClaimValue)).ToList();
        }

        public async Task<int> Insert(Claim userClaim, string userId)
        {
            return await InsertAsync(CreateInstance(userClaim, userId));
        }

        public async Task<int> InsertMany(IEnumerable<Claim> claims, string userId)
        {
            return await InsertManyAsync(claims.Select(claim => CreateInstance(claim, userId)));
        }

        private static IdentityUserClaim CreateInstance(Claim userClaim, string userId)
        {
            return new IdentityUserClaim
            {
                ClaimType = userClaim.Type,
                ClaimValue = userClaim.Value,
                UserId = userId
            };
        }
    }
}
