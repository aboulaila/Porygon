using Microsoft.AspNetCore.Identity;
using Porygon.Entity;

namespace Porygon.Identity.Entity
{
    public class IdentityUserLogin : IdentityUserLogin<string>, IKeyEntity<string>
    {
        public string? Id { get; set; }
    }
}
