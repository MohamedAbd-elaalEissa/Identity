using ApplicationContract.CustomException;
using ApplicationContract.Models;
using System.Net;
using System.Text.Json;

namespace Identity.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (status, errorResponse) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound,
                    new ErrorResponse(exception, HttpStatusCode.NotFound, _env.IsDevelopment())),

                ValidationException => (HttpStatusCode.BadRequest,
                    new ErrorResponse(exception, HttpStatusCode.BadRequest, _env.IsDevelopment())),

                UnauthorizedException => (HttpStatusCode.Unauthorized,
                    new ErrorResponse(exception, HttpStatusCode.Unauthorized, _env.IsDevelopment())),

                ForbiddenException => (HttpStatusCode.Forbidden,
                    new ErrorResponse(exception, HttpStatusCode.Forbidden, _env.IsDevelopment())),

                BadRequestException => (HttpStatusCode.BadRequest,
                    new ErrorResponse(exception, HttpStatusCode.BadRequest, _env.IsDevelopment())),

                InternalServerErrorException => (HttpStatusCode.InternalServerError,
                    new ErrorResponse(exception, HttpStatusCode.InternalServerError, _env.IsDevelopment())),

                _ => (HttpStatusCode.InternalServerError,
                    new ErrorResponse(exception, HttpStatusCode.InternalServerError, _env.IsDevelopment()))
            };

            // Log the exception
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            // Set response details
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            // Serialize and write response
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }
    }
}
