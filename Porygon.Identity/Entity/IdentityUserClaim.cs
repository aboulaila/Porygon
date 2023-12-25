using Microsoft.AspNetCore.Identity;
using Porygon.Entity;

namespace Porygon.Identity.Entity
{
    public class IdentityUserClaim : IdentityUserClaim<string>, IKeyEntity<int>
    {
        public new int? Id { get; set; }
    }
}
