using MediatR;
using Payments.Application.Orders.Queries;

namespace Payments.Application.Payments.Queries
{
    public sealed record ListPaymentsByOrderQuery(Guid OrderId) : IRequest<OrderWithPaymentsDto>;

    public sealed record OrderWithPaymentsDto(OrderDto Order,
                                              IReadOnlyList<PaymentListItemDto> Payments);

    public sealed record PaymentListItemDto(Guid Id, string Status, decimal Amount, string Currency,
                                            DateTimeOffset CreatedAt, string? ProviderPaymentId, string? FailureReason);
}

