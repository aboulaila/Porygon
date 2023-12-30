using Microsoft.AspNetCore.Identity;

namespace Porygon.Identity.Entity
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }
        [PersonalData]
        public string? MiddleName { get; set; }
        [PersonalData]
        public string? LastName { get; set; }
    }
}
