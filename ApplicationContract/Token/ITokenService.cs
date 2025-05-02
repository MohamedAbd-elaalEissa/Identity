using Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace ApplicationContract.Token
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppIdentityUser user,UserManager<AppIdentityUser> userManager);
        string GenerateRefreshToken();
    }
}
