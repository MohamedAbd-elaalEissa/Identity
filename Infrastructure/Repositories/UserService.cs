using ApplicationContract;
using ApplicationContract.CustomException;
using ApplicationContract.Models;
using ApplicationContract.Token;
using Domain.Entities;
using Infrastructure.Presistence;
using Infrastructure.Repositories.RabbitMQ;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static ApplicationContract.CustomException.NotFoundException;

namespace Infrastructure.Repositories
{
    public class UserService(UserManager<AppIdentityUser> _userManager, ITokenService _token,
        SignInManager<AppIdentityUser> _signIn, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration,
        IdentityDbContexct _context
        //, RegisterPublisher _publisher
        ) : IUserService
    {

        public async Task<UserDTO> LoginAsync(LoginDTO login)
        {
            var user = _userManager.FindByEmailAsync(login.Email).Result;
            if (user == null) { throw new UnauthorizedException("Invalid email or password"); }
            var password = await _signIn.CheckPasswordSignInAsync(user, login.Password, false);
            if (password == null) { throw new UnauthorizedException("Invalid email or password"); }
            return new UserDTO()
            {

                //UserName = user.DisplayName,
                //Email = user.Email,
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
            if (user.UserName.Contains(" "))
            {
                user.UserName = user.UserName.Replace(" ", "");
            }
            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
            {
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to create user");
            }
            /// for Rabbitmq
            //await _publisher.PublishRegisterDataAsync(user.UserName, user.Email, user.PhoneNumber, register.IsTeacher);
            await NotifyStudentApiOfRegistration(user.DisplayName, user.Email, user.PhoneNumber, register.IsTeacher,register.academicLevelId,null);
            
            return "Sucessfully Registered";
        }

        private async Task NotifyStudentApiOfRegistration(string userName, string email, string phoneNumber, bool isTeacher,int? academicLevelId,int? subjectId)
        {
            using (var httpClient = new HttpClient())
            {
                var Dto = new
                {
                    FirstName = userName?.Split(' ').FirstOrDefault(),
                    LastName = userName?.Split(' ').Skip(1).FirstOrDefault(),
                    PhoneNumber = phoneNumber,
                    Email = email,
                    AcademicLevelId = academicLevelId,
                    SubjectId= subjectId
                };
                var user = _userManager.FindByEmailAsync(email).Result;
                var Token = await _token.CreateToken(user, _userManager);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                // Make the API call through Ocelot gateway
                // Adjust the endpoint path as needed based on your Student API
                HttpResponseMessage response;
                if (isTeacher == true)
                {
                    var command = new
                    {
                        teacher = Dto
                    };
                    var content = new StringContent(
                        JsonSerializer.Serialize(command),
                        Encoding.UTF8,
                        "application/json");
                    //response = await httpClient.PostAsync("https://identitysso-001-site1.ktempurl.com/api/Teacher/CreateTeacher", content);
                    response = await httpClient.PostAsync("https://localhost:44301/api/Teacher/CreateTeacher", content);

                }
                else
                {
                    var command = new
                    {
                        student = Dto
                    };
                    var content = new StringContent(
                        JsonSerializer.Serialize(command),
                        Encoding.UTF8,
                        "application/json");
                    //response = await httpClient.PostAsync("https://identitysso-001-site1.ktempurl.com/api/Student/CreateStudent", content);
                    response = await httpClient.PostAsync("https://localhost:44301/api/Student/CreateStudent", content);

                }

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new InternalServerErrorException($"Failed to notify Student API: {response.StatusCode}, {errorContent}");
                }
            }
        }
        public Task CreateRole(RolesDTO roles)
        {
            var role = _roleManager.RoleExistsAsync(roles.RoleName).Result;
            if (role)
            {
                throw new BadRequestException("Role already exists");
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
                throw new NotFoundException("User not found");
            }
            await _userManager.AddToRoleAsync(user, userroles.RoleName);
        }

        public async Task<List<string>> GetUserRoles(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
