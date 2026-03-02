using Payments.Infrastructure.Common.Persistence.Rows;
using Payments.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Common.Persistence
{
    internal class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IUnitOfWork
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<CurrencyRow> Currencies => Set<CurrencyRow>();
        public DbSet<IdempotencyRow> Idempotency => Set<IdempotencyRow>();

        public async Task CommitChangesAsync(CancellationToken cancellationToken = default)
               => await SaveChangesAsync(cancellationToken);

        public async Task<T> InTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct)
        {
            if (Database.CurrentTransaction is not null)
                return await action(ct);

            await using var tx = await Database.BeginTransactionAsync(ct);

            try
            {
                var result = await action(ct);
                await SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                return result;
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }

}