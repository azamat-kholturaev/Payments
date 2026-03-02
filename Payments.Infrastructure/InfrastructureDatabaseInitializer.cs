using Microsoft.Extensions.DependencyInjection;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure
{
    public static class InfrastructureDatabaseInitializer
    {
        public static async Task ApplyMigrationsAndSeedAsync(IServiceProvider services, CancellationToken ct = default)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await DbSeeder.SeedAsync(db, ct);
        }
    }
}
