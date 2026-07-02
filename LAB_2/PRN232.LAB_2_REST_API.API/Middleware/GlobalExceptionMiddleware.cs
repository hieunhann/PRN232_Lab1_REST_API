using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PRN232.LAB_2_REST_API.Services.Models.Responses;
using System.Net;
using System.Text.Json;

namespace PRN232.LAB_2_REST_API.API.Middleware
{
    /// <summary>
    /// Global Exception Handling Middleware.
    /// Catches all unhandled exceptions, logs them, and returns a standardized error response.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[GlobalException] Unhandled exception at {Method} {Path}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "You do not have permission to access this resource."),
                ArgumentNullException => (HttpStatusCode.BadRequest, "Invalid input data."),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Requested resource not found."),
                _ => (HttpStatusCode.InternalServerError, "Internal server error")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Errors = null  // Hide internal exception details from the client
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
