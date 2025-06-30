using Application.Features.Identity.Commands;
using Application.Features.Identity.Queries;
using ApplicationContract.Models;
using ApplicationContract.Token;
using Domain.Entities;
using Infrastructure.Presistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    public class IdentityController(ITokenService _tokenService, SignInManager<AppIdentityUser> _signIn,
        IdentityDbContexct _context, UserManager<AppIdentityUser> _userManager, IConfiguration _configuration) : GeneralController
    {
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<string>> Register(RegisterCommand command)
        {
            try
            {
                var res = await Mediator.Send(command);
                return Ok(res);
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    message = ex.Message
                });
            }

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginCommand command)
        {
            var res = await Mediator.Send(command);
            return Ok(res);
        }

        [HttpGet]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return Ok(new { message = "Logout successful" });
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleCommand command)
        {
            var res = await Mediator.Send(command);
            return Ok(res);
        }

        [HttpPost]
        [Route("UserRole")]
        public async Task<IActionResult> UserRole(UserRoleCommand command)
        {
            var res = await Mediator.Send(command);
            return Ok(res);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto model)
        {
            if (model is null || string.IsNullOrEmpty(model.RefreshToken))
                return BadRequest("Invalid client request");
            var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == model.RefreshToken);
            if (tokenEntity == null || tokenEntity.IsExpired)
                return Unauthorized("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
            if (user == null)
                return Unauthorized("User Not Found");

            var newAccessToken = await _tokenService.CreateToken(user, _userManager);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            tokenEntity.Token = newRefreshToken;
            tokenEntity.Expires = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:RefreshTokenValidation"]));
            tokenEntity.Created = DateTime.UtcNow;

            _context.RefreshTokens.Update(tokenEntity);
            await _context.SaveChangesAsync();
            return Ok(new UserDTO()
            {

                //UserName = user.DisplayName,
                //Email = user.Email,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string email)
        {
            GetUserRolesQuery query = new GetUserRolesQuery(email);
            var res = await Mediator.Send(query);
            return Ok(res);
        }
    }
}
