using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class AppIdentityUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
