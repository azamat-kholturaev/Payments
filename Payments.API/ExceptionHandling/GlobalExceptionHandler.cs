using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Payments.API.ExceptionHandling
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
        {
            logger.LogError(ex, "Unhandled exception");

            if (ex is UnauthorizedAccessException)
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    error = new { code = "auth.unauthorized", message = "Unauthorized" },
                    traceId = ctx.TraceIdentifier
                }, ct);
                return true;
            }

            if (ex is ApplicationException appEx)
            {
                var code = string.IsNullOrWhiteSpace(appEx.Message) ? "application.error" : appEx.Message;
                var status = code switch
                {
                    var c when c.StartsWith("auth.forbidden", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status403Forbidden,
                    var c when c.StartsWith("auth.", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status401Unauthorized,
                    var c when c.EndsWith(".not_found", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status404NotFound,
                    var c when c.EndsWith(".not_payable", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status409Conflict,
                    var c when c.StartsWith("validation.", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status400BadRequest
                };

                ctx.Response.StatusCode = status;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    error = new { code, message = appEx.Message },
                    traceId = ctx.TraceIdentifier
                }, ct);
                return true;
            }

            var (status, code) = ex switch
            {
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "concurrency.conflict"),
                DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } =>
                    (StatusCodes.Status409Conflict, "conflict.unique_violation"),
                NpgsqlException => (StatusCodes.Status503ServiceUnavailable, "db.unavailable"),
                TimeoutException => (StatusCodes.Status504GatewayTimeout, "timeout"),
                HttpRequestException => (StatusCodes.Status503ServiceUnavailable, "network.unavailable"),
                TaskCanceledException => (StatusCodes.Status504GatewayTimeout, "timeout"),
                _ => (StatusCodes.Status500InternalServerError, "server.error")
            };

            ctx.Response.StatusCode = status;

            await ctx.Response.WriteAsJsonAsync(new
            {
                error = new { code, message = "Unexpected error" },
                traceId = ctx.TraceIdentifier
            }, ct);

            return true;
        }
    }
}
