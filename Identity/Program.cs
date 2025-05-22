
using Application.Shared;
using Domain.Entities;
using Identity.Shared;
using Infrastructure.IdentitySeed;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Identity;
using Presentation.Shared;

namespace Identity
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddPresentationServices();
            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var LoggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var user = services.GetRequiredService<UserManager<AppIdentityUser>>();
                if (user != null)
                {
                    await IdentityDbContexctSeed.SeedUserAsync(user);
                }
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "error In Update database");
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGlobalExceptionHandling();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.Run();
        }
    }
}
