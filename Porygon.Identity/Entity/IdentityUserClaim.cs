using FreeSql.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Porygon.Identity.Entity
{
    public class IdentityUserClaim : IdentityUserClaim<string>
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public override int Id { get => base.Id; set => base.Id = value; }
    }
}