using Payments.Application.Common.Abstractions;

namespace Payments.Application.Payments.Commands
{
    public sealed record CreatePaymentCommand(Guid OrderId, string IdempotencyKey) : ICommand<PaymentDto>;

    public sealed record PaymentDto(Guid Id, Guid OrderId, decimal Amount, string Currency, string Status, DateTimeOffset CreatedAt);
}
