using Payments.Application.Common.Interfaces;
using System.Threading.RateLimiting;
using Payments.API.Middleware;
using System.Security.Claims;
using Payments.API.Services;
using Payments.API.OpenApi;
using Scalar.AspNetCore;

namespace Payments.API
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddControllers();

            // ошибки
            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            // current user
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();


            // rate limiting policies
            services.AddRateLimitingPolicies();

            // OpenAPI + Swagger UI
            services.AddOpenApi();
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();
                options.AddOperationTransformer<AuthorizeOperationTransformer>();
            });

            return services;
        }

        public static WebApplication UsePresentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(o =>
                {
                    o.SwaggerEndpoint("/openapi/v1.json", "Payments API v1");
                    o.RoutePrefix = "swagger";
                });

                app.MapGet("/", () => Results.Redirect("/swagger"));
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            return app;
        }

        private static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // public: per IP
                options.AddPolicy("ip-public", ctx =>
                {
                    var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("token-user-commands", ctx =>
                {
                    var userId =
                        ctx.User.FindFirstValue("id")
                        ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? "anon";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"cmd:{userId}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,                 // команды
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("token-user-queries", ctx =>
                {
                    var userId =
                        ctx.User.FindFirstValue("id")
                        ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? "anon";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"qry:{userId}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,                // запросы
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });
            });

            return services;
        }
    }
}
