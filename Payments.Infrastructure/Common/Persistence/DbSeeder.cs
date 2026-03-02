using Payments.Infrastructure.Common.Persistence.Rows;
using Microsoft.EntityFrameworkCore;

namespace Payments.Infrastructure.Common.Persistence
{
    public static class DbSeeder
    {
        internal static async Task SeedAsync(AppDbContext db, CancellationToken ct)
        {
            await db.Database.MigrateAsync(ct);

            if (!await db.Currencies.AnyAsync(ct))
            {
                db.Currencies.AddRange(
                    new CurrencyRow { Code = "USD", NumericCode = 840, MinorUnits = 2, Name = "US Dollar", IsActive = true },
                    new CurrencyRow { Code = "EUR", NumericCode = 978, MinorUnits = 2, Name = "Euro", IsActive = true },
                    new CurrencyRow { Code = "TJS", NumericCode = 972, MinorUnits = 2, Name = "Somoni", IsActive = true }
                );

                await db.SaveChangesAsync(ct);
            }
        }
    }
}
