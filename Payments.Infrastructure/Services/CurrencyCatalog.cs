using Payments.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Payments.Infrastructure.Common.Persistence;

namespace Payments.Infrastructure.Services
{
    internal sealed class CurrencyCatalog(AppDbContext db, IMemoryCache cache) : ICurrencyCatalog
    {
        private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);
        private readonly AppDbContext _db = db;
        private readonly IMemoryCache _cache = cache;

        public async Task<CurrencyInfo?> GetAsync(string code, CancellationToken ct)
        {
            var normalized = (code ?? "").Trim().ToUpperInvariant();
            if (normalized.Length != 3) return null;

            var cacheKey = $"currency:{normalized}";
            if (_cache.TryGetValue<CurrencyInfo>(cacheKey, out var cached))
                return cached;

            var row = await _db.Currencies.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == normalized, ct);

            if (row is null)
                return null;

            var info = new CurrencyInfo(row.Code, row.NumericCode, row.MinorUnits, row.Name, row.IsActive);

            _cache.Set(cacheKey, info, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = Ttl
            });

            return info;
        }


        public async Task<bool> IsSupportedAsync(string code, CancellationToken ct)
            => (await GetAsync(code, ct))?.IsActive == true;
    }
}
