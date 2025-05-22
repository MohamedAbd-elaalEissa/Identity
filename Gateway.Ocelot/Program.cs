using Gateway.Ocelot.Shared;
using Microsoft.AspNetCore.DataProtection;
using Ocelot.Middleware;
using System.Security.Cryptography.X509Certificates;

namespace Gateway.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOcelotServices(builder.Configuration);
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
                 .AddJsonFile("Ocelot.json",optional: false, reloadOnChange: true)
                 .AddEnvironmentVariables();
            //builder.Services.AddDataProtection()
            //        .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
            //        .ProtectKeysWithCertificate(new X509Certificate2("/https/aspnetapp.pfx", "1998"));
            var app = builder.Build();

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOcelot().Wait();

            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.Run();
        }
    }
}
