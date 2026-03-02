using Microsoft.EntityFrameworkCore;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Repositories
{
    internal class PaymentsReadRepository(AppDbContext db): IReadRepository<Payment>
    {
        private readonly AppDbContext _db = db;

        public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
            => _db.Payments.AsNoTracking().AnyAsync(x => x.Id == id, ct);

        public IQueryable<Payment> Query()
            => _db.Payments.AsNoTracking();
    }
}
