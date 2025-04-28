using ApplicationContract;
using ApplicationContract.Models;
using ApplicationContract.Token;
using Domain.Entities;
using Infrastructure.Presistence;
using Infrastructure.Repositories.RabbitMQ;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{
    public class UserService(UserManager<AppIdentityUser> _userManager, ITokenService _token,
        SignInManager<AppIdentityUser> _signIn, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration,
        IdentityDbContexct _context, RegisterPublisher _publisher) : IUserService
    {

        public async Task<UserDTO> LoginAsync(LoginDTO login)
        {
            var user = _userManager.FindByEmailAsync(login.Email).Result;
            if (user == null) { throw new UnauthorizedAccessException("Invalid email or password"); }
            var password = await _signIn.CheckPasswordSignInAsync(user, login.Password, false);
            if (password == null) { throw new UnauthorizedAccessException("Invalid email or password"); }
            return new UserDTO()
            {

                UserName = user.DisplayName,
                Email = user.Email,
                Token = await _token.CreateToken(user, _userManager),
                RefreshToken = await RefreshTokenAsync(user.Id),
            };
        }

        public async Task<string> RefreshTokenAsync(string UserId)
        {
            var refreshTokenValidInMin = _configuration["JWT:RefreshTokenValidation"];
            var refreshToken = new RefreshToken
            {
                Token = _token.GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(double.Parse(refreshTokenValidInMin)),
                UserId = UserId,
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken.Token;
        }
        public async Task<string> RegisterAsync(RegisterDTO register)
        {
            TypeAdapterConfig<RegisterDTO, AppIdentityUser>
               .NewConfig()
               .Map(dest => dest.DisplayName, src => src.UserName)
               .IgnoreNullValues(true);
            var user = register.Adapt<AppIdentityUser>();
            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault().Description);
            }
            await _publisher.PublishRegisterDataAsync(user.UserName, user.Email, user.PhoneNumber, register.IsTeacher);
            return "Sucessfully Registered";
        }
        public Task CreateRole(RolesDTO roles)
        {
            var role = _roleManager.RoleExistsAsync(roles.RoleName).Result;
            if (role)
            {
                throw new Exception("Role already exists");
            }
            var identityRole = new IdentityRole()
            {
                Name = roles.RoleName
            };
            var result = _roleManager.CreateAsync(identityRole).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault().Description);
            }
            return Task.CompletedTask;
        }

        public async Task UserRole(UserRolesDTO userroles)
        {
            var user = _userManager.FindByEmailAsync(userroles.Email).Result;
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _userManager.AddToRoleAsync(user, userroles.RoleName);
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
