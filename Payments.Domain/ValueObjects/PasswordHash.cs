using Payments.Domain.Common;

namespace Payments.Domain.ValueObjects
{
    public record PasswordHash
    {
        public string Value { get; private set; } = default!;

        private PasswordHash() { } // для EF

        private PasswordHash(string value)
        {
            Value = value;
        }

        public static Result<PasswordHash> Create(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                return Result<PasswordHash>.Fail(new Error("password.hash_empty", "Password hash is required"));

            if (hash.Length < 20)
                return Result<PasswordHash>.Fail(new Error("password.hash_invalid", "Password hash is invalid"));

            return Result<PasswordHash>.Ok(new PasswordHash(hash));
        }

        public override string ToString() => Value;
    }
}
