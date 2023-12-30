using FreeSql.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Porygon.Identity.Entity
{
    public class User : IdentityUser
    {

        [Column(IsIdentity = true, IsPrimary = true)]
        public override string Id { get => base.Id; set => base.Id = value; }
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? MiddleName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
    }
}
