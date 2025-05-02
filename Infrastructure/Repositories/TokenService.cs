using ApplicationContract.Token;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Repositories
{
    public class TokenService(IConfiguration _configuration) : ITokenService
    {
        public async Task<string> CreateToken(AppIdentityUser user, UserManager<AppIdentityUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.DisplayName),
                new Claim("email",user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };
            var authRoles = await userManager.GetRolesAsync(user);
            foreach (var role in authRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
                // we can change this to role to can easly use it in ocelat.json
                //authClaims.Add(new Claim("Role", role));
            }
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var token = new JwtSecurityToken
                (
                 issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audiences"],
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:TokenValidation"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
