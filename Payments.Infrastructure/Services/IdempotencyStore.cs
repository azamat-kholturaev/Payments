using Payments.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Payments.Infrastructure.Common.Persistence;
using Payments.Infrastructure.Common.Persistence.Rows;

namespace Payments.Infrastructure.Services
{
    internal sealed class IdempotencyStore(AppDbContext db) : IIdempotencyStore
    {
        private readonly AppDbContext _db = db;

        public async Task<IdemResponse?> GetAsync(Guid userId, string key, string scope, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var rec = await _db.Idempotency.AsNoTracking()
                .Where(x => x.UserId == userId && x.Key == key && x.Scope == scope && x.ExpiresAt > now)
                .FirstOrDefaultAsync(ct);

            return rec is null ? null : new IdemResponse(rec.StatusCode, rec.ResponseJson);
        }

        public async Task<bool> TrySaveAsync(Guid userId, string key, string scope, int statusCode, string json, CancellationToken ct)
        {
            _db.Idempotency.Add(new IdempotencyRow
            {
                UserId = userId,
                Key = key.Trim(),
                Scope = scope.Trim(),
                StatusCode = statusCode,
                ResponseJson = json,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
            });

            try
            {
                await _db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                return false; 
            }
        }
    }
}
