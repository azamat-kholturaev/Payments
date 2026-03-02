using Payments.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Repositories
{
    internal sealed class OrdersWriteRepository(AppDbContext db) : IOrdersWriteRepository
    {
        private readonly AppDbContext _db = db;

        public async Task<Order?> GetTrackingByIdAsync(Guid id, CancellationToken ct)
            => await _db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task AddAsync(Order entity, CancellationToken ct) 
            => await _db.Orders.AddAsync(entity, ct);

        public Task<Order?> GetForUpdateAsync(Guid orderId, CancellationToken ct)
         => _db.Orders
             .FromSql($@"SELECT * FROM orders WHERE ""Id"" = {orderId} FOR UPDATE")
             .TagWith("OrdersWriteRepository.GetForUpdateAsync")
             .AsTracking()
             .FirstOrDefaultAsync(ct);

        public void Remove(Order entity)
            => _db.Orders.Remove(entity);
    }
}
