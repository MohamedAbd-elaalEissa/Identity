using Domain.Entities;
using Infrastructure.Presistence;
using Infrastructure.Repositories.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Shared
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration confegeration)
        {
            services.AddDbContext<IdentityDbContexct>(options =>
            {
                options.UseSqlServer(confegeration.GetConnectionString("IdentityConnection"),
                        sqlServerOptions =>
                        {
                            sqlServerOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null
                            );
                        });
            });
            services.AddIdentity<AppIdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityDbContexct>().AddDefaultTokenProviders(); ;
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }
            ).AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = false; //In Production (true) For more Secure
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = confegeration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = confegeration["JWT:Audiences"],
                    ClockSkew = TimeSpan.Zero,// Ensure no time skew (tokens expire exactly on time)
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confegeration["JWT:Key"]))

                };
            });
            //RabbitMQ
              //services.AddScoped<RegisterPublisher>();
            return services;
        }
    }
}
