using Payments.Domain.Common;

namespace Payments.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get; private set; }
        public Currency Currency { get; private set; } = default!;

        private Money() { } // EF

        private Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Result<Money> Create(decimal amount, string currencyCode)
        {
            if (amount <= 0)
                return Result<Money>.Fail(new Error("money.amount_invalid", "Amount must be > 0"));

            var c = Currency.Create(currencyCode);
            if (!c.IsSuccess)
                return Result<Money>.Fail(c.Error);

            return Result<Money>.Ok(new Money(amount, c.Value!));
        }
    }
}