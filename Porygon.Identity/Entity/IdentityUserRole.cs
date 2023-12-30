using FreeSql.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Porygon.Identity.Entity
{
    public class IdentityUserRole : IdentityUserRole<string>
    {
        [Column(IsIdentity = true)]
        public override string UserId { get => base.UserId; set => base.UserId = value; }
        [Column(IsIdentity = true)]
        public override string RoleId { get => base.RoleId; set => base.RoleId = value; }
    }
}
