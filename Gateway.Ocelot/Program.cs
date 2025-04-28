using Gateway.Ocelot.Shared;
using Ocelot.Middleware;

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
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOcelot().Wait();

            app.MapControllers();

            app.Run();
        }
    }
}
