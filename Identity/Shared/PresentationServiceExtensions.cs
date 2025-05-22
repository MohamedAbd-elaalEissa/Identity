using Infrastructure.Presistence;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace Presentation.Shared
{
    public static class PresentationServiceExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // Add Swagger services
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                 });
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
            return services;
        }
    }
}
