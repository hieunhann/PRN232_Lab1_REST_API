using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace PRN232.LAB_2_REST_API.API.Middleware
{
    /// <summary>
    /// Request Logging Middleware.
    /// Logs information for each request: Path, Method, Execution Time, Status Code.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Capture request info before processing
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "/";
            var requestId = context.Request.Headers["X-Request-Id"].FirstOrDefault()
                            ?? Guid.NewGuid().ToString("N")[..8];

            _logger.LogInformation(
                "[RequestLog] [{RequestId}] --> {Method} {Path}",
                requestId, method, path);

            // Continue pipeline
            await _next(context);

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            // Log after response
            _logger.LogInformation(
                "[RequestLog] [{RequestId}] <-- {Method} {Path} | Status: {StatusCode} | Time: {ElapsedMs}ms",
                requestId, method, path, statusCode, elapsed);

            // Warn if response is slow > 500ms
            if (elapsed > 500)
            {
                _logger.LogWarning(
                    "[RequestLog] [{RequestId}] SLOW REQUEST: {Method} {Path} took {ElapsedMs}ms",
                    requestId, method, path, elapsed);
            }
        }
    }
}
