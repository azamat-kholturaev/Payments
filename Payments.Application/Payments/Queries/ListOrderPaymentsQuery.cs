using MediatR;

namespace Payments.Application.Payments.Queries
{
    public sealed record ListOrderPaymentsQuery(Guid OrderId) : IRequest<IReadOnlyList<PaymentDto>>;

    public sealed record PaymentDto(
        Guid Id,
        Guid OrderId,
        decimal Amount,
        string Currency,
        string Status,
        string IdempotencyKey,
        DateTimeOffset CreatedAt,
        string? ProviderPaymentId,
        string? FailureReason);
}
