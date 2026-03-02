using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Payments.Application.Common.Exceptions;

namespace Payments.API.Middleware
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception");

            var problem = Map(exception, httpContext);

            httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }

        private static ProblemDetails Map(Exception exception, HttpContext context) => exception switch
        {
            ValidationException validation => new ProblemDetails
            {
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join("; ", validation.Errors.Select(x => x.ErrorMessage)),
                Type = "validation.failed",
                Instance = context.Request.Path
            },
            AppException app => new ProblemDetails
            {
                Title = "Business error",
                Status = app.StatusCode,
                Detail = app.Message,
                Type = app.Code,
                Instance = context.Request.Path
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Authentication required",
                Type = "auth.unauthorized",
                Instance = context.Request.Path
            },
            _ => new ProblemDetails
            {
                Title = "Server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Internal server error",
                Type = "internal.error",
                Instance = context.Request.Path
            }
        };
    }
}
