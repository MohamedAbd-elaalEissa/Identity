using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Values;
using System.Text;

namespace Gateway.Ocelot.Shared;

public static class OcelotServiceExtensions
{
    public static IServiceCollection AddOcelotServices(this IServiceCollection services, IConfiguration confegeration)
    {
        services.AddSwaggerGen();
        // allow Angular to reach to API(CORS)
        var MyAllowSpecificOrigins = "AllowAllOrigins";
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins("http://localhost:4200",
                                            "https://identitysso-001-site1.ktempurl.com",
                                            "https://orginalplatform-001-site1.mtempurl.com",
                                            "https://ssoidentity-001-site1.anytempurl.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });
        services.AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", option =>
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
        services.AddAuthorization();
        services.AddOcelot(confegeration);
        return services;
    }
}
