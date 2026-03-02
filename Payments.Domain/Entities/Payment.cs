using Payments.Domain.Common;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities
{
    public sealed class Payment : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }

        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = default!;

        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

        public IdempotencyKey IdempotencyKey { get; private set; } = default!;
        public string? ProviderPaymentId { get; private set; }
        public string? FailureReason { get; private set; }

        private Payment() { }

        private Payment(Guid orderId, Guid userId, decimal amount, string currency, IdempotencyKey idem)
        {
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            Currency = currency;
            IdempotencyKey = idem;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Result<Payment> Create(Guid orderId, Guid userId, Money money, string idempotencyKey)
        {
            if (orderId == Guid.Empty)
                return Result<Payment>.Fail(new Error("payment.order_invalid", "OrderId is invalid"));
            if (userId == Guid.Empty)
                return Result<Payment>.Fail(new Error("payment.user_invalid", "UserId is invalid"));

            var idem = IdempotencyKey.Create(idempotencyKey);
            if (!idem.IsSuccess) return Result<Payment>.Fail(idem.Error);

            return Result<Payment>.Ok(new Payment(
                orderId,
                userId,
                money.Amount,
                money.Currency.Code,
                idem.Value!
            ));
        }

        public Result EnsureOwner(Guid userId)
        {
            if (UserId != userId)
                return Result.Fail(new Error("auth.forbidden", "Not your payment"));
            return Result.Ok();
        }

        public Result MarkSuccessful(string providerPaymentId)
        {
            if (Status != PaymentStatus.Pending)
                return Result.Fail(new Error("payment.not_pending", "Only pending payment can be successful"));

            if (string.IsNullOrWhiteSpace(providerPaymentId))
                return Result.Fail(new Error("payment.provider_id_required", "ProviderPaymentId is required"));

            Status = PaymentStatus.Successful;
            ProviderPaymentId = providerPaymentId.Trim();
            return Result.Ok();
        }

        public Result MarkFailed(string reason)
        {
            if (Status != PaymentStatus.Pending)
                return Result.Fail(new Error("payment.not_pending", "Only pending payment can be failed"));

            Status = PaymentStatus.Failed;
            FailureReason = string.IsNullOrWhiteSpace(reason) ? "Unknown failure" : reason.Trim();
            return Result.Ok();
        }
    }
}
