using Payments.Domain.Common;

namespace Payments.Domain.ValueObjects
{
    public record Email
    {
        public string Value { get; private set; } = default!;

        private Email() { }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<Email>.Fail(new Error("email.empty", "Email is required"));

            var normalized = email.Trim().ToLowerInvariant();
            var at = normalized.IndexOf('@');

            if (at <= 0 || at != normalized.LastIndexOf('@') || at == normalized.Length - 1)
                return Result<Email>.Fail(new Error("email.invalid", "Email format is invalid"));

            return Result<Email>.Ok(new Email(normalized));
        }

        public override string ToString() => Value;
    }
}
