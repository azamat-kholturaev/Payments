using Payments.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Payments.Infrastructure.Common.Persistence.Converters
{
    internal static class ValueConverters
    {
        public static readonly ValueConverter<Email, string> EmailConverter =
            new(
                v => v.Value,
                v => Email.Create(v).Value!
            );

        public static readonly ValueConverter<PasswordHash, string> PasswordHashConverter =
            new(
                v => v.Value,
                v => PasswordHash.Create(v).Value!
            );

        public static readonly ValueConverter<IdempotencyKey, string> IdempotencyKeyConverter =
            new(
                v => v.Value,
                v => IdempotencyKey.Create(v).Value!
            );
    }
}
