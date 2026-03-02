using Payments.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Repositories
{
    internal sealed class OrdersReadRepository(AppDbContext db) : IReadRepository<Order>
    {
        private readonly AppDbContext _db = db;

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
            => await _db.Orders.AsNoTracking().AnyAsync(x => x.Id == id, ct);

        public IQueryable<Order> Query()
            => _db.Orders.AsNoTracking();
    }
}
