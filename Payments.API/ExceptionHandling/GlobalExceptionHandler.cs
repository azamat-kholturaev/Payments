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

            var (status, code) = ex switch
            {
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "concurrency.conflict"),
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
