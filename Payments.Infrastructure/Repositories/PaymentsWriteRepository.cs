using Microsoft.EntityFrameworkCore;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Repositories
{
    internal sealed class PaymentsWriteRepository(AppDbContext db) : IPaymentsWriteRepository
    {
        private readonly AppDbContext _db = db;

        public Task<Payment?> GetTrackingByIdAsync(Guid id, CancellationToken ct)
            => _db.Payments.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task AddAsync(Payment entity, CancellationToken ct)
            => _db.Payments.AddAsync(entity, ct).AsTask();

        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken ct)
            => await _db.Payments.AsNoTracking().Where(x => x.OrderId == orderId).ToListAsync(ct);

        public void Remove(Payment entity)
            => _db.Payments.Remove(entity);
    }
}
