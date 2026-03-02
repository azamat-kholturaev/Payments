using Payments.Domain.Common;

namespace Payments.Domain.ValueObjects
{
    public sealed record IdempotencyKey
    {
        public string Value { get; private set; } = default!;

        private IdempotencyKey() { } // EF

        private IdempotencyKey(string value) => Value = value;

        public static Result<IdempotencyKey> Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Result<IdempotencyKey>.Fail(new Error("idem.empty", "Idempotency-Key is required"));

            var k = key.Trim();

            if (k.Length > 128)
                return Result<IdempotencyKey>.Fail(new Error("idem.too_long", "Idempotency-Key is too long"));

            return Result<IdempotencyKey>.Ok(new IdempotencyKey(k));
        }

        public override string ToString() => Value;
    }
}
