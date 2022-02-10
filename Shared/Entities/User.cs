using Microsoft.AspNetCore.Identity;

namespace Shared.Entities
{
    public class User : IdentityUser
    {
        public bool Debtor { get; set; }
    }
}
