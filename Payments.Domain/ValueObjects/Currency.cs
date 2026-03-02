using Payments.Domain.Common;

namespace Payments.Domain.ValueObjects
{
    public sealed record Currency
    {
        public string Code { get; private set; } = default!;

        private Currency() { } // EF

        private Currency(string code) => Code = code;

        public static Result<Currency> Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Result<Currency>.Fail(new Error("currency.empty", "Currency is required"));

            var c = code.Trim().ToUpperInvariant();
            if (c.Length != 3)
                return Result<Currency>.Fail(new Error("currency.invalid", "Currency must be 3 letters"));

            return Result<Currency>.Ok(new Currency(c));
        }

        public override string ToString() => Code;
    }
}
