using MediatR;

namespace Payments.Application.Payments.Queries;

public sealed record ListPaymentsByOrderQuery(Guid OrderId) : IRequest<IReadOnlyList<PaymentListItemDto>>;

public sealed record PaymentListItemDto(Guid Id, string Status, decimal Amount, string Currency, DateTimeOffset CreatedAt, string? ProviderPaymentId, string? FailureReason);
