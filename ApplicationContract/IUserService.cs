using ApplicationContract.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationContract
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDTO register);
        Task<UserDTO> LoginAsync(LoginDTO login);
        Task<string> RefreshTokenAsync(string UserId);
        Task CreateRole(RolesDTO roles);
        Task UserRole(UserRolesDTO userroles);
        Task<List<string>> GetUserRoles(string email);
    }
}
