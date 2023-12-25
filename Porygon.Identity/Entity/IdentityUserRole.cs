using Microsoft.AspNetCore.Identity;
using Porygon.Entity;

namespace Porygon.Identity.Entity
{
    public class IdentityUserRole : IdentityUserRole<string>, IKeyEntity<string>
    {
    }
}
