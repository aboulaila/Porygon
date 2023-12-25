using Microsoft.AspNetCore.Identity;
using Porygon.Entity;

namespace Porygon.Identity.Entity
{
    public class User : IdentityUser, IKeyEntity<string>
    {
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? MiddleName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
    }
}
